using StarterTemplate.Domain.Entities;

namespace StarterTemplate.Application.Interfaces
{
    /// <summary>
    /// Repository interface for Role entities.
    /// Defines data access operations for role entities.
    /// </summary>
    public interface IRoleRepository : IGenericRepository<Role>
    {
        /// <summary>
        /// Retrieves a role by its name asynchronously.
        /// </summary>
        /// <param name="name">The role name to search for.</param>
        /// <returns>The role if found; otherwise, null.</returns>
        Task<Role?> GetByNameAsync(string name);
    }
}

