using StarterTemplate.Domain.Entities;
using StarterTemplate.Application.DTOs;

namespace StarterTemplate.Application.Interfaces
{
    /// <summary>
    /// Service interface for managing user entities.
    /// Provides operations for user management including CRUD operations and user-specific functionality.
    /// </summary>
    public interface IUserService : IGenericCrudService<User, UserDto>
    {
        /// <summary>
        /// Gets a user by their email address asynchronously.
        /// </summary>
        /// <param name="email">The email address to search for.</param>
        /// <returns>The user DTO if found; otherwise, null.</returns>
        Task<UserDto?> GetByEmailAsync(string email);

        /// <summary>
        /// Gets a user by their username asynchronously.
        /// </summary>
        /// <param name="userName">The username to search for.</param>
        /// <returns>The user DTO if found; otherwise, null.</returns>
        Task<UserDto?> GetByUserNameAsync(string userName);

        /// <summary>
        /// Checks if an email address is unique in the system asynchronously.
        /// </summary>
        /// <param name="email">The email address to check.</param>
        /// <returns>True if the email is unique; otherwise, false.</returns>
        Task<bool> IsEmailUniqueAsync(string email);

        /// <summary>
        /// Checks if a username is unique in the system asynchronously.
        /// </summary>
        /// <param name="userName">The username to check.</param>
        /// <returns>True if the username is unique; otherwise, false.</returns>
        Task<bool> IsUserNameUniqueAsync(string userName);

        /// <summary>
        /// Gets all available roles in the system asynchronously.
        /// </summary>
        /// <returns>A collection of all roles.</returns>
        Task<IEnumerable<RoleDto>> GetAllRolesAsync();

        /// <summary>
        /// Gets all roles assigned to a specific user asynchronously.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>A collection of roles assigned to the user.</returns>
        Task<IEnumerable<RoleDto>> GetUserRolesAsync(int userId);

        /// <summary>
        /// Assigns roles to a user asynchronously.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="roleIds">The list of role IDs to assign.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        Task<bool> AssignRolesToUserAsync(int userId, List<int> roleIds);

        /// <summary>
        /// Removes a role from a user asynchronously.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="roleId">The role ID to remove.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        Task<bool> RemoveRoleFromUserAsync(int userId, int roleId);

        /// <summary>
        /// Update the profile picture of a user asynchronously.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="newProfilePictureDto">The new profile picture as a base64 encoded string from the Data Transfer Object.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        Task<bool> UpdateProfilePictureAsync(int userId, NewUserDto newProfilePictureDto);

        /// <summary>
        /// Gets the profile picture of a user asynchronously.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>The profile picture of a user</returns>
        Task<string?> GetUserProfilePictureAsync(int userId);

        /// <summary>
        /// Update the username of a user asynchronously.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="newUserNameDto">The new username Data Transfer Object.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        Task<bool> UpdateUserNameAsync(int userId, NewUserDto newUserNameDto);

        /// <summary>
        /// Update the status of a user asynchronously.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="newStatus">The new status.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        Task<bool> UpdateUserStatusAsync(int userId, bool newStatus);
    }
}
