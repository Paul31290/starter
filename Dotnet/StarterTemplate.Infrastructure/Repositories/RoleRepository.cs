using StarterTemplate.Application.Interfaces;
using StarterTemplate.Domain.Entities;
using StarterTemplate.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace StarterTemplate.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Role entities.
    /// Provides data access operations for role entities.
    /// </summary>
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        /// <summary>
        /// Initializes a new instance of the RoleRepository class.
        /// </summary>
        /// <param name="context">The database context for data access.</param>
        public RoleRepository(StarterTemplateContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves a role by its name asynchronously.
        /// </summary>
        /// <param name="name">The role name to search for.</param>
        /// <returns>The role if found; otherwise, null.</returns>
        public async Task<Role?> GetByNameAsync(string name)
        {
            return await _context.Set<Role>()
                .FirstOrDefaultAsync(r => r.Name == name);
        }
    }
}

