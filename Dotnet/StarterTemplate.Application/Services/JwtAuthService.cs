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
using Microsoft.AspNetCore.WebUtilities;

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
        private readonly EmailService _emailService;

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
            IStarterTemplateContext context,
            EmailService emailService)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _jwtSettings = jwtSettings.Value;
            _context = context;
            _emailService = emailService;
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

        /// <summary>
        /// Sends a password reset link to the specified email address.
        /// </summary>
        /// <param name="email">The email address of the user requesting the password reset.</param>
        /// <returns>A task that represents the asynchronous operation, with a boolean result indicating success or failure.</returns>
        public async Task<bool> SendPasswordResetLinkAsync(string email)
        {
            // Try to find the user by their email address
            var user = await _userRepository.GetByEmailAsync(email);
            // Security measure: 
            // Do not reveal whether the user exists or not — 
            // always behave the same if the user is not found or the email is not confirmed
            if (user == null)
                return false;
            // Get user roles
            var userRoles = await _userRoleRepository.GetByUserIdAsync(user.Id);
            var roles = userRoles.Where(ur => ur.Role != null).Select(ur => ur.Role!.Name).ToList();

            // Generate a unique, secure token for password reset
            var token = GenerateAccessToken(user.Id, user.UserName, user.Email, roles);
            // Send the reset link via email to the user
            await _emailService.SendPasswordResetEmailAsync(user.Email, token, user.UserName);
            return true;
        }
        /// <summary>
        /// Resets the user's password using the provided reset token and new password.
        /// </summary>
        /// <param name="resetPasswordDto">The DTO containing the reset password information.</param>
        /// <returns>A task that represents the asynchronous operation, with a ValidationResultDto indicating success or failure.</returns>
        public async Task<ValidationResultDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {

            var failed = new ValidationResultDto { IsValid = false };

            if (resetPasswordDto == null || string.IsNullOrWhiteSpace(resetPasswordDto.Email) || string.IsNullOrWhiteSpace(resetPasswordDto.Token) || string.IsNullOrWhiteSpace(resetPasswordDto.Password) || string.IsNullOrWhiteSpace(resetPasswordDto.ConfirmPassword))
            {
                return failed;
            }
            // Try to find the user by their email address
            var user = await _userRepository.GetByEmailAsync(resetPasswordDto.Email);

            // If user not found, return a generic failure (no details leaked for security)
            if (user == null)
                return failed;

            // Decode the token that was passed in from the reset link
            var decodedBytes = WebEncoders.Base64UrlDecode(resetPasswordDto.Token);
            var decodedToken = Encoding.UTF8.GetString(decodedBytes);
            
            // Validate the token as a JWT
            var principal = await ValidateTokenAsync(decodedToken);
            if (principal == null)
                return failed;

            // Hash and set the new password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.Password);
            var updated = _userRepository.UpdateAsync(user);
            if (updated != null)
                return new ValidationResultDto { IsValid = true };

            return failed;
        }
    }
}
