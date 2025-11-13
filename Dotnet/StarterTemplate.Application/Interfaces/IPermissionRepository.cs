using StarterTemplate.Domain.Entities;
using StarterTemplate.Application.Interfaces;

namespace StarterTemplate.Application.Repositories
{
    /// <summary>
    /// Repository interface for Permission entities.
    /// Extends the generic repository to provide permission-specific data access operations.
    /// </summary>
    public interface IPermissionRepository : IGenericRepository<Permission>
    {
        /// <summary>
        /// Gets all permissions for a specific role.
        /// </summary>
        /// <param name="roleId">The ID of the role.</param>
        /// <returns>A collection of permissions assigned to the role.</returns>
        Task<IEnumerable<Permission>> GetPermissionsByRoleIdAsync(int roleId);

        /// <summary>
        /// Gets all permissions for a specific user (through their roles).
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A collection of permissions the user has through their roles.</returns>
        Task<IEnumerable<Permission>> GetPermissionsByUserIdAsync(int userId);

        /// <summary>
        /// Checks if a user has a specific permission.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="permissionName">The name of the permission to check.</param>
        /// <returns>True if the user has the permission, otherwise false.</returns>
        Task<bool> UserHasPermissionAsync(int userId, string permissionName);
    }
}

