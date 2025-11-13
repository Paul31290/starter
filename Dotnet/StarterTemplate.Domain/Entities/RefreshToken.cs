using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StarterTemplate.Domain.Entities
{
    /// <summary>
    /// Represents a refresh token entity for JWT authentication.
    /// Used to store refresh tokens for token renewal functionality.
    /// </summary>
    public class RefreshToken : BaseEntity
    {
        /// <summary>
        /// Gets or sets the refresh token value.
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user ID associated with this refresh token.
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the expiration date and time of the refresh token.
        /// </summary>
        [Required]
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the refresh token has been revoked.
        /// </summary>
        [Required]
        public bool IsRevoked { get; set; } = false;

        /// <summary>
        /// Gets or sets the date and time when the refresh token was revoked.
        /// </summary>
        public DateTime? RevokedAt { get; set; }

        /// <summary>
        /// Gets or sets the IP address from which the refresh token was created.
        /// </summary>
        [StringLength(45)]
        public string? CreatedByIp { get; set; }

        /// <summary>
        /// Gets or sets the IP address from which the refresh token was revoked.
        /// </summary>
        [StringLength(45)]
        public string? RevokedByIp { get; set; }

        /// <summary>
        /// Gets or sets the reason for revoking the refresh token.
        /// </summary>
        [StringLength(200)]
        public string? RevocationReason { get; set; }

        /// <summary>
        /// Gets or sets the replacement token if this token was replaced.
        /// </summary>
        [StringLength(500)]
        public string? ReplacedByToken { get; set; }

        /// <summary>
        /// Navigation property to the user associated with this refresh token.
        /// </summary>
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}
