using Microsoft.AspNetCore.Mvc;
using StarterTemplate.Application.DTOs;
using StarterTemplate.Application.Interfaces;
using StarterTemplate.Api.Controllers;
using System.Text.Json;
using System.Runtime;

namespace StarterTemplate.Api.Controllers
{
    /// <summary>
    /// Controller for managing application settings.
    /// Provides endpoints for retrieving and updating application configuration settings.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SettingsController : BaseApiController
    {
        private readonly ISettingsService _settingsService;

        private readonly IUserService _userService;

        public SettingsController(ISettingsService settingsService, IUserService userService)
        {
            _settingsService = settingsService;
            _userService = userService;
        }

        /// <summary>
        /// Gets a setting value by key.
        /// </summary>
        /// <param name="key">The setting key.</param>
        /// <param name="userId">Optional user ID for user-specific settings.</param>
        /// <returns>The setting value if found.</returns>
        [HttpGet("{key}")]
        public async Task<ActionResult<string?>> GetSettingValue(string key, [FromQuery] int? userId = null)
        {
            try
            {
                var value = await _settingsService.GetSettingValueAsync(key, userId);
                return Ok(value);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Sets a setting value by key.
        /// </summary>
        /// <param name="key">The setting key.</param>
        /// <param name="request">The setting value and metadata.</param>
        /// <returns>The updated or created setting.</returns>
        [HttpPut("{key}")]
        public async Task<ActionResult<SettingsDto>> SetSettingValue(string key, [FromBody] SetSettingRequestDto request)
        {
            try
            {
                var setting = await _settingsService.SetSettingValueAsync(
                    key, 
                    request.Value, 
                    request.Description, 
                    request.Category, 
                    request.UserId);
                
                return Ok(setting);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Gets all settings for a specific category.
        /// </summary>
        /// <param name="category">The setting category.</param>
        /// <param name="userId">Optional user ID for user-specific settings.</param>
        /// <returns>A list of settings in the category.</returns>
        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<SettingsDto>>> GetSettingsByCategory(string category, [FromQuery] int? userId = null)
        {
            try
            {
                var settings = await _settingsService.GetSettingsByCategoryAsync(category, userId);
                return Ok(settings);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Gets all global settings (not user-specific).
        /// </summary>
        /// <returns>A list of global settings.</returns>
        [HttpGet("global")]
        public async Task<ActionResult<IEnumerable<SettingsDto>>> GetGlobalSettings()
        {
            try
            {
                var settings = await _settingsService.GetGlobalSettingsAsync();
                return Ok(settings);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Gets all user-specific settings.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>A list of user-specific settings.</returns>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<SettingsDto>>> GetUserSettings(int userId)
        {
            try
            {
                var settings = await _settingsService.GetUserSettingsAsync(userId);
                return Ok(settings);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Checks if a setting key exists.
        /// </summary>
        /// <param name="key">The setting key.</param>
        /// <param name="userId">Optional user ID for user-specific settings.</param>
        /// <returns>True if the setting exists; otherwise, false.</returns>
        [HttpGet("{key}/exists")]
        public async Task<ActionResult<bool>> SettingExists(string key, [FromQuery] int? userId = null)
        {
            try
            {
                var exists = await _settingsService.SettingExistsAsync(key, userId);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Deletes a setting by key.
        /// </summary>
        /// <param name="key">The setting key.</param>
        /// <param name="userId">Optional user ID for user-specific settings.</param>
        /// <returns>True if the setting was deleted; otherwise, false.</returns>
        [HttpDelete("{key}")]
        public async Task<ActionResult<bool>> DeleteSetting(string key, [FromQuery] int? userId = null)
        {
            try
            {
                var deleted = await _settingsService.DeleteSettingAsync(key, userId);
                return Ok(deleted);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Gets a typed setting value.
        /// </summary>
        /// <param name="key">The setting key.</param>
        /// <param name="type">The type to convert the value to.</param>
        /// <param name="defaultValue">The default value if the setting is not found.</param>
        /// <param name="userId">Optional user ID for user-specific settings.</param>
        /// <returns>The typed setting value or default value.</returns>
        [HttpGet("{key}/typed")]
        public async Task<ActionResult<object>> GetTypedSetting(string key, [FromQuery] string type, [FromQuery] string? defaultValue = null, [FromQuery] int? userId = null)
        {
            try
            {
                object result = type.ToLowerInvariant() switch
                {
                    "int" or "integer" => await _settingsService.GetTypedSettingAsync(key, defaultValue != null ? int.Parse(defaultValue) : 0, userId),
                    "bool" or "boolean" => await _settingsService.GetTypedSettingAsync(key, defaultValue != null ? bool.Parse(defaultValue) : false, userId),
                    "double" => await _settingsService.GetTypedSettingAsync(key, defaultValue != null ? double.Parse(defaultValue) : 0.0, userId),
                    "decimal" => await _settingsService.GetTypedSettingAsync(key, defaultValue != null ? decimal.Parse(defaultValue) : 0m, userId),
                    "datetime" => await _settingsService.GetTypedSettingAsync(key, defaultValue != null ? DateTime.Parse(defaultValue) : DateTime.MinValue, userId),
                    _ => await _settingsService.GetSettingValueAsync(key, userId) ?? defaultValue ?? string.Empty
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        
        /// <summary>
        /// Updates the username of a user.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <param name="newUserNameDto">The new username Data Transfer Object.</param>
        /// <returns>The updated user.</returns>
        [HttpPut("{id}/change-username")]
        public async Task<ActionResult<UserDto>> ChangeUserName(int id, [FromBody] NewUserDto newUserNameDto)
        {
            var user = await _userService.GetByIdAsync(id);

            // Verify current username
            if (user == null)
            {
                return NotFound();
            }

            // Check if new username is unique
            if (!await _userService.IsUserNameUniqueAsync(newUserNameDto.NewUserName))
            {
                return BadRequest("Username is already in use.");
            }

            var updatedUser = await _userService.UpdateUserNameAsync(id, newUserNameDto);
            return Ok(updatedUser);
        }

        /// <summary>
        /// Post the profile picture URL of a user.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <param name="newProfilePictureDto">The new profile picture as a base64 encoded string from the Data Transfer Object.</param>
        /// <returns>The new information of the user.</returns>
        [HttpPut("{id}/change-profile-picture")]
        public async Task<ActionResult<UserDto>> ChangeProfilePicture(int id,[FromBody] NewUserDto newProfilePictureDto)
        {
            var user = await _userService.GetByIdAsync(id);

            // Verify username
            if (user == null)
            {
                return NotFound();
            }

            var updatedUser = await _userService.UpdateProfilePictureAsync(id, newProfilePictureDto);
            return Ok(updatedUser);
        }

        /// <summary>
        /// Updates the status of a user.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <param name="newStatus">The new status.</param>
        /// <returns>The updated user.</returns>
        [HttpPut("{id}/change-status")]
        public async Task<ActionResult<UserDto>> ChangeUserStatus(int id, [FromBody] bool newStatus)
        {
            var user = await _userService.GetByIdAsync(id);

            // Verify username
            if (user == null)
            {
                return NotFound();
            }

            var updatedUser = await _userService.UpdateUserStatusAsync(id, newStatus);
            return Ok(updatedUser);
        }

        /// <summary>
        /// Gets the profile picture URL of a user.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>The updated user.</returns>
        [HttpGet("{id}/get-profile-picture")]
        public async Task<ActionResult<string>> GetProfilePicture(int id)
        {
            var user = await _userService.GetByIdAsync(id);
                // Verify user
            if (user == null)
            {
                return NotFound();
            }
            var profilePicture = await _userService.GetUserProfilePictureAsync(user.Id);
            if (profilePicture is not null){
                return Ok(profilePicture);
            } else{
                return NotFound();
            }
        } 

    }

    /// <summary>
    /// Data Transfer Object for setting a setting value.
    /// </summary>
    public class SetSettingRequestDto
    {
        /// <summary>
        /// Gets or sets the setting value.
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the optional description for the setting.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the setting category.
        /// </summary>
        public string Category { get; set; } = "General";

        /// <summary>
        /// Gets or sets the optional user ID for user-specific settings.
        /// </summary>
        public int? UserId { get; set; }
    }
}
