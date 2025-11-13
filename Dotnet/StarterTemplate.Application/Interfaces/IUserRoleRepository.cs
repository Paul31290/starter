using StarterTemplate.Domain.Entities;

namespace StarterTemplate.Application.Interfaces
{
    /// <summary>
    /// Repository interface for UserRole entities.
    /// Defines data access operations for user-role relationships.
    /// </summary>
    public interface IUserRoleRepository : IGenericRepository<UserRole>
    {
        /// <summary>
        /// Retrieves all roles for a specific user asynchronously.
        /// </summary>
        /// <param name="userId">The user ID to search for.</param>
        /// <returns>A collection of user roles.</returns>
        Task<IEnumerable<UserRole>> GetByUserIdAsync(int userId);

        /// <summary>
        /// Retrieves all users with a specific role asynchronously.
        /// </summary>
        /// <param name="roleId">The role ID to search for.</param>
        /// <returns>A collection of user roles.</returns>
        Task<IEnumerable<UserRole>> GetByRoleIdAsync(int roleId);
    }
}

