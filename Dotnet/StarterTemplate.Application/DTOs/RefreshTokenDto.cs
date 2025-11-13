using System.ComponentModel.DataAnnotations;

namespace StarterTemplate.Application.DTOs
{
    /// <summary>
    /// DTO for refresh token request.
    /// Contains the refresh token used to obtain a new access token.
    /// </summary>
    public class RefreshTokenDto
    {
        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        [Required(ErrorMessage = "Refresh token is required.")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
