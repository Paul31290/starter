using StarterTemplate.Domain.Entities;

namespace StarterTemplate.Application.Interfaces
{
    /// <summary>
    /// Repository interface for User entities.
    /// Extends the generic repository to provide user-specific data access operations including authentication-related queries.
    /// </summary>
    public interface IUserRepository : IGenericRepository<User>
    {
        /// <summary>
        /// Retrieves a user by their email address asynchronously.
        /// </summary>
        /// <param name="email">The email address to search for.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        Task<User?> GetByEmailAsync(string email);
        
        /// <summary>
        /// Retrieves a user by their username asynchronously.
        /// </summary>
        /// <param name="userName">The username to search for.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        Task<User?> GetByUserNameAsync(string userName);
        
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
    }
} 