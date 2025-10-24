using StarterTemplate.Application.DTOs;
using StarterTemplate.Domain.Entities;

namespace StarterTemplate.Application.Mappings
{
    /// <summary>
    /// Mapper class for converting between Settings entities and SettingsDto objects.
    /// Handles the mapping of application settings data.
    /// </summary>
    public class SettingsMapper : IEntityMapper<Settings, SettingsDto>
    {
        /// <summary>
        /// Converts a Settings entity to a SettingsDto object.
        /// </summary>
        /// <param name="entity">The Settings entity to convert.</param>
        /// <returns>A SettingsDto object containing the mapped data.</returns>
        public SettingsDto ToDto(Settings entity)
        {
            return new SettingsDto
            {
                Id = entity.Id,
                Key = entity.Key,
                Value = entity.Value,
                Description = entity.Description,
                Category = entity.Category,
                IsEncrypted = entity.IsEncrypted,
                IsReadOnly = entity.IsReadOnly,
                UserId = entity.UserId,
                CreatedAt = entity.CreatedAt,
                CreatedById = entity.CreatedById,
                ModifiedAt = entity.ModifiedAt,
                ModifiedById = entity.ModifiedById
            };
        }

        /// <summary>
        /// Converts a SettingsDto object to a Settings entity.
        /// </summary>
        /// <param name="dto">The SettingsDto object to convert.</param>
        /// <returns>A Settings entity containing the mapped data.</returns>
        public Settings ToEntity(SettingsDto dto)
        {
            return new Settings
            {
                Id = dto.Id,
                Key = dto.Key,
                Value = dto.Value,
                Description = dto.Description,
                Category = dto.Category,
                IsEncrypted = dto.IsEncrypted,
                IsReadOnly = dto.IsReadOnly,
                UserId = dto.UserId,
                CreatedAt = dto.CreatedAt,
                CreatedById = dto.CreatedById,
                ModifiedAt = dto.ModifiedAt,
                ModifiedById = dto.ModifiedById
            };
        }

        /// <summary>
        /// Updates an existing Settings entity with data from a SettingsDto object.
        /// </summary>
        /// <param name="entity">The Settings entity to update.</param>
        /// <param name="dto">The SettingsDto object containing the updated data.</param>
        public void UpdateEntity(Settings entity, SettingsDto dto)
        {
            entity.Key = dto.Key;
            entity.Value = dto.Value;
            entity.Description = dto.Description;
            entity.Category = dto.Category;
            entity.IsEncrypted = dto.IsEncrypted;
            entity.IsReadOnly = dto.IsReadOnly;
            entity.UserId = dto.UserId;
            entity.ModifiedAt = dto.ModifiedAt;
            entity.ModifiedById = dto.ModifiedById;
        }
    }
}
