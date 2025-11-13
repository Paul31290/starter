using StarterTemplate.Domain.Entities;
using StarterTemplate.Application.Repositories;
using StarterTemplate.Infrastructure.Data;
using StarterTemplate.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace StarterTemplate.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for RolePermission entities.
    /// Provides data access operations for role-permission entities using Entity Framework Core.
    /// </summary>
    public class RolePermissionRepository : GenericRepository<RolePermission>, IRolePermissionRepository
    {
        /// <summary>
        /// Initializes a new instance of the RolePermissionRepository class.
        /// </summary>
        /// <param name="context">The database context for data access.</param>
        public RolePermissionRepository(StarterTemplateContext context) : base(context) { }

        /// <summary>
        /// Assigns a permission to a role.
        /// </summary>
        /// <param name="roleId">The ID of the role.</param>
        /// <param name="permissionId">The ID of the permission.</param>
        /// <returns>The created RolePermission entity.</returns>
        public async Task<RolePermission> AssignPermissionToRoleAsync(int roleId, int permissionId)
        {
            var rolePermission = new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _context.Set<RolePermission>().AddAsync(rolePermission);
            await _context.SaveChangesAsync();

            return rolePermission;
        }

        /// <summary>
        /// Removes a permission from a role.
        /// </summary>
        /// <param name="roleId">The ID of the role.</param>
        /// <param name="permissionId">The ID of the permission.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public async Task<bool> RemovePermissionFromRoleAsync(int roleId, int permissionId)
        {
            var rolePermission = await _context.Set<RolePermission>()
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);

            if (rolePermission == null)
                return false;

            _context.Set<RolePermission>().Remove(rolePermission);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Gets all role permissions for a specific role.
        /// </summary>
        /// <param name="roleId">The ID of the role.</param>
        /// <returns>A collection of role permissions.</returns>
        public async Task<IEnumerable<RolePermission>> GetRolePermissionsByRoleIdAsync(int roleId)
        {
            return await _context.Set<RolePermission>()
                .Where(rp => rp.RoleId == roleId)
                .Include(rp => rp.Permission)
                .ToListAsync();
        }
    }
}

