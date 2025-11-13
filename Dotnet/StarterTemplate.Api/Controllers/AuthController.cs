using Microsoft.AspNetCore.Mvc;
using StarterTemplate.Application.DTOs;
using StarterTemplate.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using ASPNETCoreIdentityDemo.ViewModels;

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
        /// <summary>
        /// Renders the forgot password view.
        /// </summary>
        /// <returns> The forgot password view.</returns>
        [HttpGet("forgot-password")]
        public IActionResult ForgotPassword()
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during the retrieving of the password");
                return StatusCode(500, new { message = "An error occurred during the password recovery." });
            }
        }

        /// <summary>
        /// Handles the forgot password request by sending a reset link to the user's email.
        /// </summary>
        /// <param name="forgotPasswordDto"></param>
        /// <returns>A Result indicating the success or failure of the operation.</returns>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var emailResult = await _jwtAuthService.SendPasswordResetLinkAsync(forgotPasswordDto.Email);
                // Always show confirmation view (don’t reveal if email exists)
                return Ok(emailResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during the email transmission");
                return StatusCode(500, new { message = "An error occurred during the sending of the reset password." });
            }
        }
        /// <summary>
        /// Renders the reset password screen
        /// </summary>
        /// <param name="email">The email of the user resetting the password</param>
        /// <param name="token">The generated token for the password reset process</param>
        /// <returns>The reset password screen with the email and token</returns>
        [HttpGet("reset-password")]
        public IActionResult ResetPassword(string email, [FromQuery] string token)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
                    return BadRequest("Invalid password reset request.");
                return Ok(new ResetPasswordDto { Email = email, Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during the password reset request");
                return StatusCode(500, new { message = "An error occurred during the request of the reset password." });
            }
        }

        /// <summary>
        /// Handles the reset password request by updating the user's password.
        /// </summary>
        /// <param name="resetPasswordDto">The reset password information.</param>
        /// <returns>A Result indicating the success or failure of the operation.</returns>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var result = await _jwtAuthService.ResetPasswordAsync(resetPasswordDto);
                if (result.IsValid)
                    return Ok(result);
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.ErrorMessage);
                return Ok(resetPasswordDto);
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while resetting the password");
                return StatusCode(500, new { message = "An error occurred while resetting the password." });
            }
        }
    }
}
