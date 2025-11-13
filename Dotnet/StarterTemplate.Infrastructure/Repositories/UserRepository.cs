using StarterTemplate.Application.Interfaces;
using StarterTemplate.Domain.Entities;
using StarterTemplate.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace StarterTemplate.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for User entities.
    /// Provides data access operations for user entities including authentication-related queries.
    /// </summary>
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        /// <summary>
        /// Initializes a new instance of the UserRepository class.
        /// </summary>
        /// <param name="context">The database context for data access.</param>
        public UserRepository(StarterTemplateContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves a user by their email address asynchronously.
        /// Includes the user's roles.
        /// </summary>
        /// <param name="email">The email address to search for.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Set<User>()
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <summary>
        /// Retrieves a user by their username asynchronously.
        /// Includes the user's roles.
        /// </summary>
        /// <param name="userName">The username to search for.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        public async Task<User?> GetByUserNameAsync(string userName)
        {
            return await _context.Set<User>()
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserName == userName);
        }

        /// <summary>
        /// Checks if an email address is unique in the system asynchronously.
        /// </summary>
        /// <param name="email">The email address to check.</param>
        /// <returns>True if the email is unique; otherwise, false.</returns>
        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            return !await _context.Set<User>()
                .AnyAsync(u => u.Email == email);
        }

        /// <summary>
        /// Checks if a username is unique in the system asynchronously.
        /// </summary>
        /// <param name="userName">The username to check.</param>
        /// <returns>True if the username is unique; otherwise, false.</returns>
        public async Task<bool> IsUserNameUniqueAsync(string userName)
        {
            return !await _context.Set<User>()
                .AnyAsync(u => u.UserName == userName);
        }
    }
} 