using StarterTemplate.Application.Interfaces;
using StarterTemplate.Domain.Entities;
using StarterTemplate.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace StarterTemplate.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for UserRole entities.
    /// Provides data access operations for user-role relationships.
    /// </summary>
    public class UserRoleRepository : GenericRepository<UserRole>, IUserRoleRepository
    {
        /// <summary>
        /// Initializes a new instance of the UserRoleRepository class.
        /// </summary>
        /// <param name="context">The database context for data access.</param>
        public UserRoleRepository(StarterTemplateContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves all roles for a specific user asynchronously.
        /// </summary>
        /// <param name="userId">The user ID to search for.</param>
        /// <returns>A collection of user roles.</returns>
        public async Task<IEnumerable<UserRole>> GetByUserIdAsync(int userId)
        {
            return await _context.Set<UserRole>()
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all users with a specific role asynchronously.
        /// </summary>
        /// <param name="roleId">The role ID to search for.</param>
        /// <returns>A collection of user roles.</returns>
        public async Task<IEnumerable<UserRole>> GetByRoleIdAsync(int roleId)
        {
            return await _context.Set<UserRole>()
                .Include(ur => ur.User)
                .Where(ur => ur.RoleId == roleId)
                .ToListAsync();
        }
    }
}

