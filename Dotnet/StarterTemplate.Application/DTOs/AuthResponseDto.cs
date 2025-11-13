namespace StarterTemplate.Application.DTOs
{
    /// <summary>
    /// DTO for authentication response.
    /// Contains the tokens and user information returned after successful authentication.
    /// </summary>
    public class AuthResponseDto
    {
        /// <summary>
        /// Gets or sets the JWT access token.
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the refresh token for token renewal.
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the token type (typically "Bearer").
        /// </summary>
        public string TokenType { get; set; } = "Bearer";

        /// <summary>
        /// Gets or sets the expiration time of the access token in seconds.
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Gets or sets the authenticated user information.
        /// </summary>
        public UserDto User { get; set; } = new UserDto();
    }
}
