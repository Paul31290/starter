using System.ComponentModel.DataAnnotations;

namespace StarterTemplate.Application.DTOs
{
    /// <summary>
    /// DTO for user login request.
    /// Contains the credentials required for user authentication.
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Gets or sets the username or email address for login.
        /// </summary>
        [Required(ErrorMessage = "Username or email is required.")]
        [StringLength(100, ErrorMessage = "Username or email cannot exceed 100 characters.")]
        public string UsernameOrEmail { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's password.
        /// </summary>
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 255 characters.")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether to remember the user's login.
        /// </summary>
        public bool RememberMe { get; set; } = false;
    }
}
