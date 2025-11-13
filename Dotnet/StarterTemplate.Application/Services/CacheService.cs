using StarterTemplate.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace StarterTemplate.Application.Services
{
    /// <summary>
    /// Service for handling cache operations using in-memory caching.
    /// Provides methods for storing, retrieving, and managing cached data.
    /// </summary>
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CacheService> _logger;
        private readonly CacheSettings _cacheSettings;

        public CacheService(IMemoryCache memoryCache, ILogger<CacheService> logger, IOptions<CacheSettings> cacheSettings)
        {
            _memoryCache = memoryCache;
            _logger = logger;
            _cacheSettings = cacheSettings.Value;
        }

        /// <summary>
        /// Gets a value from the cache asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the cached value.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <returns>The cached value if found; otherwise, default(T).</returns>
        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                if (_memoryCache.TryGetValue(key, out var value))
                {
                    _logger.LogDebug("Cache hit for key: {Key}", key);
                    return (T?)value;
                }

                _logger.LogDebug("Cache miss for key: {Key}", key);
                return default(T);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving value from cache for key: {Key}", key);
                return default(T);
            }
        }

        /// <summary>
        /// Sets a value in the cache asynchronously with default expiration.
        /// </summary>
        /// <typeparam name="T">The type of the value to cache.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The value to cache.</param>
        /// <returns>True if the value was set successfully; otherwise, false.</returns>
        public async Task<bool> SetAsync<T>(string key, T value)
        {
            return await SetAsync(key, value, TimeSpan.FromMinutes(_cacheSettings.DefaultExpirationMinutes));
        }

        /// <summary>
        /// Sets a value in the cache asynchronously with custom expiration.
        /// </summary>
        /// <typeparam name="T">The type of the value to cache.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The value to cache.</param>
        /// <param name="expiration">The expiration time for the cached value.</param>
        /// <returns>True if the value was set successfully; otherwise, false.</returns>
        public async Task<bool> SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            try
            {
                var options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration,
                    SlidingExpiration = TimeSpan.FromMinutes(5), // Reset expiration if accessed within 5 minutes
                    Priority = CacheItemPriority.Normal
                };

                _memoryCache.Set(key, value, options);
                _logger.LogDebug("Value cached successfully for key: {Key} with expiration: {Expiration}", key, expiration);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting value in cache for key: {Key}", key);
                return false;
            }
        }

        /// <summary>
        /// Sets a value in the cache asynchronously with absolute expiration.
        /// </summary>
        /// <typeparam name="T">The type of the value to cache.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The value to cache.</param>
        /// <param name="expiresAt">The absolute expiration time.</param>
        /// <returns>True if the value was set successfully; otherwise, false.</returns>
        public async Task<bool> SetAsync<T>(string key, T value, DateTimeOffset expiresAt)
        {
            try
            {
                var options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = expiresAt,
                    Priority = CacheItemPriority.Normal
                };

                _memoryCache.Set(key, value, options);
                _logger.LogDebug("Value cached successfully for key: {Key} with absolute expiration: {ExpiresAt}", key, expiresAt);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting value in cache for key: {Key}", key);
                return false;
            }
        }

        /// <summary>
        /// Removes a value from the cache asynchronously.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <returns>True if the value was removed successfully; otherwise, false.</returns>
        public async Task<bool> RemoveAsync(string key)
        {
            try
            {
                _memoryCache.Remove(key);
                _logger.LogDebug("Value removed from cache for key: {Key}", key);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing value from cache for key: {Key}", key);
                return false;
            }
        }

        /// <summary>
        /// Removes multiple values from the cache asynchronously.
        /// </summary>
        /// <param name="keys">The cache keys to remove.</param>
        /// <returns>The number of keys that were removed.</returns>
        public async Task<int> RemoveAsync(string[] keys)
        {
            var removedCount = 0;
            
            foreach (var key in keys)
            {
                if (await RemoveAsync(key))
                {
                    removedCount++;
                }
            }

            _logger.LogDebug("Removed {Count} values from cache", removedCount);
            return removedCount;
        }

        /// <summary>
        /// Checks if a key exists in the cache.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <returns>True if the key exists; otherwise, false.</returns>
        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                return _memoryCache.TryGetValue(key, out _);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking cache existence for key: {Key}", key);
                return false;
            }
        }

        /// <summary>
        /// Gets or sets a value in the cache asynchronously.
        /// If the key doesn't exist, the factory function is called to create the value.
        /// </summary>
        /// <typeparam name="T">The type of the cached value.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="factory">The factory function to create the value if it doesn't exist.</param>
        /// <param name="expiration">Optional expiration time for the cached value.</param>
        /// <returns>The cached or newly created value.</returns>
        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            try
            {
                var cachedValue = await GetAsync<T>(key);
                if (cachedValue != null)
                {
                    return cachedValue;
                }

                var newValue = await factory();
                var expirationTime = expiration ?? TimeSpan.FromMinutes(_cacheSettings.DefaultExpirationMinutes);
                await SetAsync(key, newValue, expirationTime);
                
                return newValue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOrSet for key: {Key}", key);
                throw;
            }
        }

        /// <summary>
        /// Clears all cached values asynchronously.
        /// Note: This is not directly supported by IMemoryCache, so we'll log a warning.
        /// </summary>
        /// <returns>True if the cache was cleared successfully; otherwise, false.</returns>
        public async Task<bool> ClearAsync()
        {
            try
            {
                // IMemoryCache doesn't support clearing all entries directly
                // In a real implementation, you might want to use a different cache provider
                // or implement a custom solution to track and clear all keys
                _logger.LogWarning("ClearAsync is not fully supported by IMemoryCache. Consider using a different cache provider for full cache management.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cache");
                return false;
            }
        }

        /// <summary>
        /// Gets information about the cache.
        /// </summary>
        /// <returns>A string containing cache statistics and information.</returns>
        public async Task<string> GetCacheInfoAsync()
        {
            try
            {
                var info = new
                {
                    Provider = "In-Memory Cache",
                    DefaultExpirationMinutes = _cacheSettings.DefaultExpirationMinutes,
                    Timestamp = DateTime.UtcNow
                };

                return JsonSerializer.Serialize(info, new JsonSerializerOptions { WriteIndented = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cache info");
                return "Error retrieving cache information";
            }
        }
    }

    /// <summary>
    /// Configuration settings for cache service.
    /// </summary>
    public class CacheSettings
    {
        public string Provider { get; set; } = "Memory";
        public int DefaultExpirationMinutes { get; set; } = 30;
        public string? RedisConnectionString { get; set; }
    }
}
