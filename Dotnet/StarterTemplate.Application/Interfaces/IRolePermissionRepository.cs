using StarterTemplate.Domain.Entities;
using StarterTemplate.Application.Interfaces;

namespace StarterTemplate.Application.Repositories
{
    /// <summary>
    /// Repository interface for RolePermission entities.
    /// Extends the generic repository to provide role-permission-specific data access operations.
    /// </summary>
    public interface IRolePermissionRepository : IGenericRepository<RolePermission>
    {
        /// <summary>
        /// Assigns a permission to a role.
        /// </summary>
        /// <param name="roleId">The ID of the role.</param>
        /// <param name="permissionId">The ID of the permission.</param>
        /// <returns>The created RolePermission entity.</returns>
        Task<RolePermission> AssignPermissionToRoleAsync(int roleId, int permissionId);

        /// <summary>
        /// Removes a permission from a role.
        /// </summary>
        /// <param name="roleId">The ID of the role.</param>
        /// <param name="permissionId">The ID of the permission.</param>
        /// <returns>True if successful, otherwise false.</returns>
        Task<bool> RemovePermissionFromRoleAsync(int roleId, int permissionId);

        /// <summary>
        /// Gets all role permissions for a specific role.
        /// </summary>
        /// <param name="roleId">The ID of the role.</param>
        /// <returns>A collection of role permissions.</returns>
        Task<IEnumerable<RolePermission>> GetRolePermissionsByRoleIdAsync(int roleId);
    }
}

