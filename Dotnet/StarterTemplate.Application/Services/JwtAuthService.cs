using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using StarterTemplate.Application.DTOs;
using StarterTemplate.Application.Interfaces;
using StarterTemplate.Domain.Entities;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace StarterTemplate.Application.Services
{
    /// <summary>
    /// Service for JWT authentication operations.
    /// Handles JWT token generation, validation, and user authentication.
    /// </summary>
    public class JwtAuthService : IJwtAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly JwtSettingsDto _jwtSettings;
        private readonly IStarterTemplateContext _context;

        /// <summary>
        /// Initializes a new instance of the JwtAuthService class.
        /// </summary>
        /// <param name="userRepository">The user repository for user operations.</param>
        /// <param name="userRoleRepository">The user role repository for role operations.</param>
        /// <param name="jwtSettings">The JWT configuration settings.</param>
        /// <param name="context">The database context for data access.</param>
        public JwtAuthService(
            IUserRepository userRepository,
            IUserRoleRepository userRoleRepository,
            IOptions<JwtSettingsDto> jwtSettings,
            IStarterTemplateContext context)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _jwtSettings = jwtSettings.Value;
            _context = context;
        }

        /// <summary>
        /// Authenticates a user with username/email and password.
        /// </summary>
        /// <param name="loginDto">The login credentials.</param>
        /// <returns>Authentication response with tokens and user info if successful; otherwise, null.</returns>
        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            // Find user by username or email
            var user = await _userRepository.GetByEmailAsync(loginDto.UsernameOrEmail) ??
                      await _userRepository.GetByUserNameAsync(loginDto.UsernameOrEmail);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return null;
            }

            // Get user roles
            var userRoles = await _userRoleRepository.GetByUserIdAsync(user.Id);
            var roles = userRoles.Where(ur => ur.Role != null).Select(ur => ur.Role!.Name).ToList();

            // Generate tokens
            var accessToken = GenerateAccessToken(user.Id, user.UserName, user.Email, roles);
            var refreshToken = GenerateRefreshToken();

            // Store refresh token in database
            await StoreRefreshTokenAsync(user.Id, refreshToken);

            // Update last login
            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TokenType = "Bearer",
                ExpiresIn = _jwtSettings.ExpiryMinutes * 60,
                User = new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CreatedAt = user.CreatedAt,
                    LastLoginAt = user.LastLoginAt,
                    IsActive = user.IsActive,
                    Roles = roles
                }
            };
        }

        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <param name="registerDto">The registration information.</param>
        /// <returns>Authentication response with tokens and user info if successful; otherwise, null.</returns>
        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto)
        {
            // Check if email is already in use
            if (!await _userRepository.IsEmailUniqueAsync(registerDto.Email))
            {
                return null;
            }

            // Check if username is already in use
            if (!await _userRepository.IsUserNameUniqueAsync(registerDto.UserName))
            {
                return null;
            }

            // Create new user
            var user = new User
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            // Assign default role (User)
            var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            if (defaultRole != null)
            {
                var userRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = defaultRole.Id,
                    AssignedAt = DateTime.UtcNow
                };
                await _userRoleRepository.AddAsync(userRole);
                await _userRoleRepository.SaveChangesAsync();
            }

            // Generate tokens
            var accessToken = GenerateAccessToken(user.Id, user.UserName, user.Email, new List<string> { "User" });
            var refreshToken = GenerateRefreshToken();

            // Store refresh token
            await StoreRefreshTokenAsync(user.Id, refreshToken);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TokenType = "Bearer",
                ExpiresIn = _jwtSettings.ExpiryMinutes * 60,
                User = new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CreatedAt = user.CreatedAt,
                    IsActive = user.IsActive,
                    Roles = new List<string> { "User" }
                }
            };
        }

        /// <summary>
        /// Refreshes an access token using a refresh token.
        /// </summary>
        /// <param name="refreshTokenDto">The refresh token information.</param>
        /// <returns>New authentication response with tokens if successful; otherwise, null.</returns>
        public async Task<AuthResponseDto?> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            if (!await ValidateRefreshTokenAsync(refreshTokenDto.RefreshToken))
            {
                return null;
            }

            // Get user from refresh token
            var refreshTokenEntity = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshTokenDto.RefreshToken);

            if (refreshTokenEntity?.User == null)
            {
                return null;
            }

            var user = refreshTokenEntity.User;

            // Get user roles
            var userRoles = await _userRoleRepository.GetByUserIdAsync(user.Id);
            var roles = userRoles.Where(ur => ur.Role != null).Select(ur => ur.Role!.Name).ToList();

            // Generate new tokens
            var newAccessToken = GenerateAccessToken(user.Id, user.UserName, user.Email, roles);
            var newRefreshToken = GenerateRefreshToken();

            // Update refresh token
            await RevokeRefreshTokenAsync(refreshTokenDto.RefreshToken);
            await StoreRefreshTokenAsync(user.Id, newRefreshToken);

            return new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                TokenType = "Bearer",
                ExpiresIn = _jwtSettings.ExpiryMinutes * 60,
                User = new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CreatedAt = user.CreatedAt,
                    LastLoginAt = user.LastLoginAt,
                    IsActive = user.IsActive,
                    Roles = roles
                }
            };
        }

        /// <summary>
        /// Revokes a refresh token, effectively logging out the user.
        /// </summary>
        /// <param name="refreshToken">The refresh token to revoke.</param>
        /// <returns>True if the token was revoked successfully; otherwise, false.</returns>
        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
        {
            var tokenEntity = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (tokenEntity == null)
            {
                return false;
            }

            _context.RefreshTokens.Remove(tokenEntity);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Validates a JWT token and returns the user ID if valid.
        /// </summary>
        /// <param name="token">The JWT token to validate.</param>
        /// <returns>The user ID if the token is valid; otherwise, null.</returns>
        public async Task<int?> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
                {
                    return userId;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Generates a new JWT access token for a user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="userName">The username.</param>
        /// <param name="email">The user's email.</param>
        /// <param name="roles">The user's roles.</param>
        /// <returns>The generated JWT token.</returns>
        public string GenerateAccessToken(int userId, string userName, string email, IEnumerable<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Name, userName),
                new(ClaimTypes.Email, email)
            };

            // Add role claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Generates a new refresh token.
        /// </summary>
        /// <returns>The generated refresh token.</returns>
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        /// <summary>
        /// Validates a refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token to validate.</param>
        /// <returns>True if the refresh token is valid; otherwise, false.</returns>
        public async Task<bool> ValidateRefreshTokenAsync(string refreshToken)
        {
            var tokenEntity = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && 
                                         rt.ExpiresAt > DateTime.UtcNow && 
                                         !rt.IsRevoked);

            return tokenEntity != null;
        }

        /// <summary>
        /// Stores a refresh token in the database.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="refreshToken">The refresh token to store.</param>
        private async Task StoreRefreshTokenAsync(int userId, string refreshToken)
        {
            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();
        }
    }
}
