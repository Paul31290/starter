using System;
using System.Threading.Tasks;

namespace StarterTemplate.Application.Interfaces
{
    /// <summary>
    /// Interface for cache operations.
    /// Provides methods for storing, retrieving, and managing cached data.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Gets a value from the cache asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the cached value.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <returns>The cached value if found; otherwise, default(T).</returns>
        Task<T?> GetAsync<T>(string key);

        /// <summary>
        /// Sets a value in the cache asynchronously with default expiration.
        /// </summary>
        /// <typeparam name="T">The type of the value to cache.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The value to cache.</param>
        /// <returns>True if the value was set successfully; otherwise, false.</returns>
        Task<bool> SetAsync<T>(string key, T value);

        /// <summary>
        /// Sets a value in the cache asynchronously with custom expiration.
        /// </summary>
        /// <typeparam name="T">The type of the value to cache.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The value to cache.</param>
        /// <param name="expiration">The expiration time for the cached value.</param>
        /// <returns>True if the value was set successfully; otherwise, false.</returns>
        Task<bool> SetAsync<T>(string key, T value, TimeSpan expiration);

        /// <summary>
        /// Sets a value in the cache asynchronously with absolute expiration.
        /// </summary>
        /// <typeparam name="T">The type of the value to cache.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The value to cache.</param>
        /// <param name="expiresAt">The absolute expiration time.</param>
        /// <returns>True if the value was set successfully; otherwise, false.</returns>
        Task<bool> SetAsync<T>(string key, T value, DateTimeOffset expiresAt);

        /// <summary>
        /// Removes a value from the cache asynchronously.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <returns>True if the value was removed successfully; otherwise, false.</returns>
        Task<bool> RemoveAsync(string key);

        /// <summary>
        /// Removes multiple values from the cache asynchronously.
        /// </summary>
        /// <param name="keys">The cache keys to remove.</param>
        /// <returns>The number of keys that were removed.</returns>
        Task<int> RemoveAsync(string[] keys);

        /// <summary>
        /// Checks if a key exists in the cache.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <returns>True if the key exists; otherwise, false.</returns>
        Task<bool> ExistsAsync(string key);

        /// <summary>
        /// Gets or sets a value in the cache asynchronously.
        /// If the key doesn't exist, the factory function is called to create the value.
        /// </summary>
        /// <typeparam name="T">The type of the cached value.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="factory">The factory function to create the value if it doesn't exist.</param>
        /// <param name="expiration">Optional expiration time for the cached value.</param>
        /// <returns>The cached or newly created value.</returns>
        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);

        /// <summary>
        /// Clears all cached values asynchronously.
        /// </summary>
        /// <returns>True if the cache was cleared successfully; otherwise, false.</returns>
        Task<bool> ClearAsync();

        /// <summary>
        /// Gets information about the cache.
        /// </summary>
        /// <returns>A string containing cache statistics and information.</returns>
        Task<string> GetCacheInfoAsync();
    }
}
