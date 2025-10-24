using StarterTemplate.Application.DTOs;

namespace StarterTemplate.Application.Interfaces
{
    /// <summary>
    /// Interface for settings service operations.
    /// Provides methods for managing application settings and configuration.
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>
        /// Gets a setting value by key.
        /// </summary>
        /// <param name="key">The setting key.</param>
        /// <param name="userId">Optional user ID for user-specific settings.</param>
        /// <returns>The setting value if found; otherwise, null.</returns>
        Task<string?> GetSettingValueAsync(string key, int? userId = null);

        /// <summary>
        /// Sets a setting value by key.
        /// </summary>
        /// <param name="key">The setting key.</param>
        /// <param name="value">The setting value.</param>
        /// <param name="description">Optional description for the setting.</param>
        /// <param name="category">The setting category.</param>
        /// <param name="userId">Optional user ID for user-specific settings.</param>
        /// <returns>The updated or created setting.</returns>
        Task<SettingsDto> SetSettingValueAsync(
            string key, 
            string value, 
            string? description = null, 
            string category = "General", 
            int? userId = null);

        /// <summary>
        /// Gets all settings for a specific category.
        /// </summary>
        /// <param name="category">The setting category.</param>
        /// <param name="userId">Optional user ID for user-specific settings.</param>
        /// <returns>A list of settings in the category.</returns>
        Task<IEnumerable<SettingsDto>> GetSettingsByCategoryAsync(string category, int? userId = null);

        /// <summary>
        /// Gets all global settings (not user-specific).
        /// </summary>
        /// <returns>A list of global settings.</returns>
        Task<IEnumerable<SettingsDto>> GetGlobalSettingsAsync();

        /// <summary>
        /// Gets all user-specific settings.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>A list of user-specific settings.</returns>
        Task<IEnumerable<SettingsDto>> GetUserSettingsAsync(int userId);

        /// <summary>
        /// Checks if a setting key exists.
        /// </summary>
        /// <param name="key">The setting key.</param>
        /// <param name="userId">Optional user ID for user-specific settings.</param>
        /// <returns>True if the setting exists; otherwise, false.</returns>
        Task<bool> SettingExistsAsync(string key, int? userId = null);

        /// <summary>
        /// Deletes a setting by key.
        /// </summary>
        /// <param name="key">The setting key.</param>
        /// <param name="userId">Optional user ID for user-specific settings.</param>
        /// <returns>True if the setting was deleted; otherwise, false.</returns>
        Task<bool> DeleteSettingAsync(string key, int? userId = null);

        /// <summary>
        /// Gets a typed setting value.
        /// </summary>
        /// <typeparam name="T">The type to convert the value to.</typeparam>
        /// <param name="key">The setting key.</param>
        /// <param name="defaultValue">The default value if the setting is not found.</param>
        /// <param name="userId">Optional user ID for user-specific settings.</param>
        /// <returns>The typed setting value or default value.</returns>
        Task<T> GetTypedSettingAsync<T>(string key, T defaultValue, int? userId = null);

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
        Task<SettingsDto> SetTypedSettingAsync<T>(
            string key, 
            T value, 
            string? description = null, 
            string category = "General", 
            int? userId = null);
    }
}
