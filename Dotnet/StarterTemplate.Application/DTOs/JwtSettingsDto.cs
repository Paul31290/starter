namespace StarterTemplate.Application.DTOs
{
    /// <summary>
    /// DTO for JWT configuration settings.
    /// Contains all necessary settings for JWT token generation and validation.
    /// </summary>
    public class JwtSettingsDto
    {
        /// <summary>
        /// Gets or sets the secret key used for signing JWT tokens.
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the issuer of the JWT tokens.
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the audience for the JWT tokens.
        /// </summary>
        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the expiry time for access tokens in minutes.
        /// </summary>
        public int ExpiryMinutes { get; set; } = 60;

        /// <summary>
        /// Gets or sets the expiry time for refresh tokens in days.
        /// </summary>
        public int RefreshTokenExpiryDays { get; set; } = 7;
    }
}
