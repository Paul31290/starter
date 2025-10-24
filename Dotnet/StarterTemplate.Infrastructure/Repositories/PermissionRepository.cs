using StarterTemplate.Domain.Entities;
using StarterTemplate.Application.Repositories;
using StarterTemplate.Infrastructure.Data;
using StarterTemplate.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace StarterTemplate.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Permission entities.
    /// Provides data access operations for permission entities using Entity Framework Core.
    /// </summary>
    public class PermissionRepository : GenericRepository<Permission>, IPermissionRepository
    {
        /// <summary>
        /// Initializes a new instance of the PermissionRepository class.
        /// </summary>
        /// <param name="context">The database context for data access.</param>
        public PermissionRepository(StarterTemplateContext context) : base(context) { }

        /// <summary>
        /// Gets all permissions for a specific role.
        /// </summary>
        /// <param name="roleId">The ID of the role.</param>
        /// <returns>A collection of permissions assigned to the role.</returns>
        public async Task<IEnumerable<Permission>> GetPermissionsByRoleIdAsync(int roleId)
        {
            return await _context.Set<RolePermission>()
                .Where(rp => rp.RoleId == roleId)
                .Include(rp => rp.Permission)
                .Select(rp => rp.Permission)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all permissions for a specific user (through their roles).
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A collection of permissions the user has through their roles.</returns>
        public async Task<IEnumerable<Permission>> GetPermissionsByUserIdAsync(int userId)
        {
            return await _context.Set<UserRole>()
                .Where(ur => ur.UserId == userId)
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.Permission)
                .Distinct()
                .ToListAsync();
        }

        /// <summary>
        /// Checks if a user has a specific permission.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="permissionName">The name of the permission to check.</param>
        /// <returns>True if the user has the permission, otherwise false.</returns>
        public async Task<bool> UserHasPermissionAsync(int userId, string permissionName)
        {
            return await _context.Set<UserRole>()
                .Where(ur => ur.UserId == userId)
                .AnyAsync(ur => ur.Role.RolePermissions
                    .Any(rp => rp.Permission.Name == permissionName));
        }
    }
}

