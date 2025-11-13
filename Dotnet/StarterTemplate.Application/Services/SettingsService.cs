using StarterTemplate.Application.DTOs;
using StarterTemplate.Application.Interfaces;
using StarterTemplate.Application.Services.GenericCrudService;
using StarterTemplate.Application.Mappings;
using StarterTemplate.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace StarterTemplate.Application.Services
{
    /// <summary>
    /// Service for managing application settings.
    /// Provides methods for retrieving and updating application configuration settings.
    /// </summary>
    public class SettingsService : GenericCrudService<Settings, SettingsDto, SettingsMapper>, ISettingsService
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly ILogger<SettingsService> _logger;

        public SettingsService(
            ISettingsRepository repository,
            SettingsMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<SettingsService> logger) : base(repository, mapper, httpContextAccessor)
        {
            _settingsRepository = repository;
            _logger = logger;
        }

        /// <summary>
        /// Gets a setting value by key.
        /// </summary>
        /// <param name="key">The setting key.</param>
        /// <param name="userId">Optional user ID for user-specific settings.</param>
        /// <returns>The setting value if found; otherwise, null.</returns>
        public async Task<string?> GetSettingValueAsync(string key, int? userId = null)
        {
            try
            {
                var setting = await _settingsRepository.GetByKeyAsync(key, userId);
                return setting?.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving setting value for key: {Key}", key);
                return null;
            }
        }

        /// <summary>
        /// Sets a setting value by key.
        /// </summary>
        /// <param name="key">The setting key.</param>
        /// <param name="value">The setting value.</param>
        /// <param name="description">Optional description for the setting.</param>
        /// <param name="category">The setting category.</param>
        /// <param name="userId">Optional user ID for user-specific settings.</param>
        /// <returns>The updated or created setting.</returns>
        public async Task<SettingsDto> SetSettingValueAsync(
            string key, 
            string value, 
            string? description = null, 
            string category = "General", 
            int? userId = null)
        {
            try
            {
                var setting = await _settingsRepository.SetValueAsync(key, value, description, category, userId);
                await _settingsRepository.SaveChangesAsync();

                _logger.LogInformation("Setting updated: {Key} = {Value}", key, value);

                return new SettingsDto
                {
                    Id = setting.Id,
                    Key = setting.Key,
                    Value = setting.Value,
                    Description = setting.Description,
                    Category = setting.Category,
                    IsEncrypted = setting.IsEncrypted,
                    IsReadOnly = setting.IsReadOnly,
                    UserId = setting.UserId,
                    CreatedAt = setting.CreatedAt,
                    CreatedById = setting.CreatedById,
                    ModifiedAt = setting.ModifiedAt,
                    ModifiedById = setting.ModifiedById
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting value for key: {Key}", key);
                throw;
            }
        }

        /// <summary>
        /// Gets all settings for a specific category.
        /// </summary>
        /// <param name="category">The setting category.</param>
        /// <param name="userId">Optional user ID for user-specific settings.</param>
        /// <returns>A list of settings in the category.</returns>
        public async Task<IEnumerable<SettingsDto>> GetSettingsByCategoryAsync(string category, int? userId = null)
        {
            try
            {
                var settings = await _settingsRepository.GetByCategoryAsync(category, userId);
                return settings.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving settings for category: {Category}", category);
                throw;
            }
        }

        /// <summary>
        /// Gets all global settings (not user-specific).
        /// </summary>
        /// <returns>A list of global settings.</returns>
        public async Task<IEnumerable<SettingsDto>> GetGlobalSettingsAsync()
        {
            try
            {
                var settings = await _settingsRepository.GetGlobalSettingsAsync();
                return settings.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving global settings");
                throw;
            }
        }

        /// <summary>
        /// Gets all user-specific settings.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>A list of user-specific settings.</returns>
        public async Task<IEnumerable<SettingsDto>> GetUserSettingsAsync(int userId)
        {
            try
            {
                var settings = await _settingsRepository.GetUserSettingsAsync(userId);
                return settings.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving settings for user: {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Checks if a setting key exists.
        /// </summary>
        /// <param name="key">The setting key.</param>
        /// <param name="userId">Optional user ID for user-specific settings.</param>
        /// <returns>True if the setting exists; otherwise, false.</returns>
        public async Task<bool> SettingExistsAsync(string key, int? userId = null)
        {
            try
            {
                return await _settingsRepository.KeyExistsAsync(key, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if setting exists: {Key}", key);
                return false;
            }
        }

        /// <summary>
        /// Deletes a setting by key.
        /// </summary>
        /// <param name="key">The setting key.</param>
        /// <param name="userId">Optional user ID for user-specific settings.</param>
        /// <returns>True if the setting was deleted; otherwise, false.</returns>
        public async Task<bool> DeleteSettingAsync(string key, int? userId = null)
        {
            try
            {
                var setting = await _settingsRepository.GetByKeyAsync(key, userId);
                if (setting != null)
                {
                    _settingsRepository.Remove(setting);
                    await _settingsRepository.SaveChangesAsync();
                    
                    _logger.LogInformation("Setting deleted: {Key}", key);
                    return true;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting setting: {Key}", key);
                return false;
            }
        }

        /// <summary>
        /// Gets a typed setting value.
        /// </summary>
        /// <typeparam name="T">The type to convert the value to.</typeparam>
        /// <param name="key">The setting key.</param>
        /// <param name="defaultValue">The default value if the setting is not found.</param>
        /// <param name="userId">Optional user ID for user-specific settings.</param>
        /// <returns>The typed setting value or default value.</returns>
        public async Task<T> GetTypedSettingAsync<T>(string key, T defaultValue, int? userId = null)
        {
            try
            {
                var value = await GetSettingValueAsync(key, userId);
                if (string.IsNullOrEmpty(value))
                    return defaultValue;

                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting typed setting: {Key}", key);
                return defaultValue;
            }
        }

        /// <summary>
        /// Sets a typed setting value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="key">The setting key.</param>
        /// <param name="value">The setting value.</param>
        /// <param name="description">Optional description for the setting.</param>
        /// <param name="category">The setting category.</param>
        /// <param name="userId">Optional user ID for user-specific settings.</param>
        /// <returns>The updated or created setting.</returns>
        public async Task<SettingsDto> SetTypedSettingAsync<T>(
            string key, 
            T value, 
            string? description = null, 
            string category = "General", 
            int? userId = null)
        {
            var stringValue = value?.ToString() ?? string.Empty;
            return await SetSettingValueAsync(key, stringValue, description, category, userId);
        }

        /// <summary>
        /// Maps a Settings entity to a SettingsDto.
        /// </summary>
        /// <param name="setting">The settings entity.</param>
        /// <returns>The mapped settings DTO.</returns>
        private static SettingsDto MapToDto(Settings setting)
        {
            return new SettingsDto
            {
                Id = setting.Id,
                Key = setting.Key,
                Value = setting.Value,
                Description = setting.Description,
                Category = setting.Category,
                IsEncrypted = setting.IsEncrypted,
                IsReadOnly = setting.IsReadOnly,
                UserId = setting.UserId,
                CreatedAt = setting.CreatedAt,
                CreatedById = setting.CreatedById,
                ModifiedAt = setting.ModifiedAt,
                ModifiedById = setting.ModifiedById
            };
        }
    }
}
