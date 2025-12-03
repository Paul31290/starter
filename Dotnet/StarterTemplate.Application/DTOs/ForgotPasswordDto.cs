using System.ComponentModel.DataAnnotations;

namespace StarterTemplate.Application.DTOs
{
    public class ForgotPasswordDto
    {
        /// <summary>
        // Gets or sets the email address of the user requesting the reset password
        /// </summary>
        [Required(ErrorMessage = "Please enter your Email address.")]
        [EmailAddress(ErrorMessage = "The Email address is not valid.")]
        public string Email { get; set; } = null!;
    }
}