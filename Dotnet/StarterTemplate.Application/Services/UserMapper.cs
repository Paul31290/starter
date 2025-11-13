using StarterTemplate.Domain.Entities;
using StarterTemplate.Application.DTOs;
using StarterTemplate.Application.Interfaces;

namespace StarterTemplate.Application.Mappings
{
    /// <summary>
    /// Mapper class for converting between User entities and UserDto objects.
    /// Handles the mapping of user data including username, email, and personal information.
    /// Note: Password information is not included in the DTO for security reasons.
    /// </summary>
    public class UserMapper : IEntityMapper<User, UserDto>
    {
        /// <summary>
        /// Converts a User entity to a UserDto object.
        /// </summary>
        /// <param name="entity">The User entity to convert.</param>
        /// <returns>A UserDto object containing the mapped data.</returns>
        public UserDto ToDto(User entity)
        {
            var roles = entity.UserRoles?
                .Where(ur => ur.Role != null)
                .Select(ur => ur.Role!.Name)
                .ToList() ?? new List<string>();

            return new UserDto
            {
                Id = entity.Id,
                UserName = entity.UserName,
                Email = entity.Email,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                CreatedAt = entity.CreatedAt,
                CreatedById = entity.CreatedById,
                ModifiedAt = entity.ModifiedAt,
                ModifiedById = entity.ModifiedById,
                LastLoginAt = entity.LastLoginAt,
                IsActive = entity.IsActive,
                Roles = roles
            };
        }

        /// <summary>
        /// Converts a UserDto object to a User entity.
        /// </summary>
        /// <param name="dto">The UserDto object to convert.</param>
        /// <returns>A User entity containing the mapped data.</returns>
        public User ToEntity(UserDto dto)
        {
            return new User
            {
                Id = dto.Id,
                UserName = dto.UserName,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                CreatedAt = dto.CreatedAt,
                CreatedById = dto.CreatedById,
                ModifiedAt = dto.ModifiedAt,
                ModifiedById = dto.ModifiedById,
                LastLoginAt = dto.LastLoginAt,
                IsActive = dto.IsActive,
                PasswordHash = string.Empty // Password should be handled separately for security
            };
        }

        /// <summary>
        /// Updates an existing User entity with data from a UserDto object.
        /// </summary>
        /// <param name="entity">The User entity to update.</param>
        /// <param name="dto">The UserDto object containing the updated data.</param>
        public void UpdateEntity(User entity, UserDto dto)
        {
            entity.UserName = dto.UserName;
            entity.Email = dto.Email;
            entity.FirstName = dto.FirstName;
            entity.LastName = dto.LastName;
            entity.LastLoginAt = dto.LastLoginAt;
            entity.IsActive = dto.IsActive;
            // Password is not updated here for security reasons
        }
    }
}
