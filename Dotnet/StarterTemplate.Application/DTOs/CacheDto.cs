using System.ComponentModel.DataAnnotations;

namespace StarterTemplate.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for cache operations.
    /// Used for setting and retrieving cached data.
    /// </summary>
    public class CacheDto
    {
        /// <summary>
        /// Gets or sets the cache key.
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the cached value.
        /// </summary>
        [Required]
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the expiration time in minutes.
        /// </summary>
        [Range(1, 1440, ErrorMessage = "Expiration must be between 1 and 1440 minutes (24 hours)")]
        public int ExpirationMinutes { get; set; } = 30;

        /// <summary>
        /// Gets or sets the absolute expiration date and time.
        /// </summary>
        public DateTime? ExpiresAt { get; set; }
    }

    /// <summary>
    /// Data Transfer Object for cache retrieval operations.
    /// Used for getting cached data by key.
    /// </summary>
    public class CacheGetDto
    {
        /// <summary>
        /// Gets or sets the cache key to retrieve.
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Key { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data Transfer Object for cache response.
    /// Contains the cached value and metadata.
    /// </summary>
    public class CacheResponseDto
    {
        /// <summary>
        /// Gets or sets the cache key.
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the cached value.
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the key exists in cache.
        /// </summary>
        public bool Exists { get; set; }

        /// <summary>
        /// Gets or sets the expiration date and time.
        /// </summary>
        public DateTime? ExpiresAt { get; set; }

        /// <summary>
        /// Gets or sets the time when the value was cached.
        /// </summary>
        public DateTime? CachedAt { get; set; }
    }

    /// <summary>
    /// Data Transfer Object for bulk cache operations.
    /// Used for setting or removing multiple cache entries.
    /// </summary>
    public class BulkCacheDto
    {
        /// <summary>
        /// Gets or sets the list of cache keys to operate on.
        /// </summary>
        [Required]
        [MinLength(1, ErrorMessage = "At least one cache key is required")]
        public List<string> Keys { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the expiration time in minutes for bulk operations.
        /// </summary>
        [Range(1, 1440, ErrorMessage = "Expiration must be between 1 and 1440 minutes (24 hours)")]
        public int ExpirationMinutes { get; set; } = 30;
    }
}
