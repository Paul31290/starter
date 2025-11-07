using System.ComponentModel.DataAnnotations;

namespace StarterTemplate.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for resetting passwords.
    /// Used for users who reset their password when they have forgotten their passwords.
    /// </summary>
    public class ResetPasswordDto
    {
        /// <summary>
        /// Gets or sets the email address of the user requesting the reset password
        /// </summary>
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid Email address.")]
        public string Email { get; set; } = null!;

        /// <summary>
        /// Gets or sets the new password of the user
        /// </summary>
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password, ErrorMessage = "Invalid Password format.")]
        public string Password { get; set; } = null!;

        /// <summary>
        /// Gets or sets the confirmed new password of the user
        /// The confirmed password must match the new password
        /// </summary>
        [Required(ErrorMessage = "Please confirm your password.")]
        [DataType(DataType.Password, ErrorMessage = "Invalid Password format.")]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password must match.")]
        public string ConfirmPassword { get; set; } = null!;

        /// <summary>
        /// Gets or sets the generated token for the password reset process
        /// </summary>
        [Required(ErrorMessage = "The password reset token is required.")]
        public string Token { get; set; } = null!;
    }
}