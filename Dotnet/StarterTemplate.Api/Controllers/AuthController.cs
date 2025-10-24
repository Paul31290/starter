using Microsoft.AspNetCore.Mvc;
using StarterTemplate.Application.DTOs;
using StarterTemplate.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace StarterTemplate.Api.Controllers
{
    /// <summary>
    /// Controller for authentication operations.
    /// Provides endpoints for user login, registration, token refresh, and logout.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtAuthService _jwtAuthService;
        private readonly ILogger<AuthController> _logger;

        /// <summary>
        /// Initializes a new instance of the AuthController class.
        /// </summary>
        /// <param name="jwtAuthService">The JWT authentication service.</param>
        /// <param name="logger">The logger for logging operations.</param>
        public AuthController(IJwtAuthService jwtAuthService, ILogger<AuthController> logger)
        {
            _jwtAuthService = jwtAuthService;
            _logger = logger;
        }

        /// <summary>
        /// Authenticates a user with username/email and password.
        /// </summary>
        /// <param name="loginDto">The login credentials.</param>
        /// <returns>Authentication response with tokens and user info if successful.</returns>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _jwtAuthService.LoginAsync(loginDto);
                if (result == null)
                {
                    return Unauthorized(new { message = "Invalid username/email or password." });
                }

                _logger.LogInformation("User {Username} logged in successfully.", loginDto.UsernameOrEmail);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login for user {Username}", loginDto.UsernameOrEmail);
                return StatusCode(500, new { message = "An error occurred during login." });
            }
        }

        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <param name="registerDto">The registration information.</param>
        /// <returns>Authentication response with tokens and user info if successful.</returns>
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _jwtAuthService.RegisterAsync(registerDto);
                if (result == null)
                {
                    return BadRequest(new { message = "Registration failed. Email or username may already be in use." });
                }

                _logger.LogInformation("User {Username} registered successfully.", registerDto.UserName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration for user {Username}", registerDto.UserName);
                return StatusCode(500, new { message = "An error occurred during registration." });
            }
        }

        /// <summary>
        /// Refreshes an access token using a refresh token.
        /// </summary>
        /// <param name="refreshTokenDto">The refresh token information.</param>
        /// <returns>New authentication response with tokens if successful.</returns>
        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _jwtAuthService.RefreshTokenAsync(refreshTokenDto);
                if (result == null)
                {
                    return Unauthorized(new { message = "Invalid or expired refresh token." });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during token refresh");
                return StatusCode(500, new { message = "An error occurred during token refresh." });
            }
        }

        /// <summary>
        /// Logs out a user by revoking their refresh token.
        /// </summary>
        /// <param name="refreshTokenDto">The refresh token to revoke.</param>
        /// <returns>Success message if logout was successful.</returns>
        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> Logout([FromBody] RefreshTokenDto refreshTokenDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var success = await _jwtAuthService.RevokeRefreshTokenAsync(refreshTokenDto.RefreshToken);
                if (!success)
                {
                    return BadRequest(new { message = "Invalid refresh token." });
                }

                _logger.LogInformation("User logged out successfully.");
                return Ok(new { message = "Logged out successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during logout");
                return StatusCode(500, new { message = "An error occurred during logout." });
            }
        }

        /// <summary>
        /// Validates a JWT token and returns user information.
        /// </summary>
        /// <returns>User information if token is valid.</returns>
        [HttpGet("validate")]
        [Authorize]
        public async Task<ActionResult> ValidateToken()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var userName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                var roles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList();

                if (userId == null)
                {
                    return Unauthorized(new { message = "Invalid token." });
                }

                return Ok(new
                {
                    userId = int.Parse(userId),
                    userName,
                    email,
                    roles,
                    isValid = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during token validation");
                return StatusCode(500, new { message = "An error occurred during token validation." });
            }
        }
    }
}
