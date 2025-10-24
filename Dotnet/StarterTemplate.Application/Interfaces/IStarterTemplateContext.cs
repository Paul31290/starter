using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using StarterTemplate.Domain.Entities;

namespace StarterTemplate.Application.Interfaces
{
    /// <summary>
    /// Interface for the application's database context.
    /// Provides access to Entity Framework Core functionality for data persistence operations.
    /// </summary>
    public interface IStarterTemplateContext
    {
        /// <summary>
        /// Gets the Users DbSet.
        /// </summary>
        DbSet<User> Users { get; }
        
        /// <summary>
        /// Gets the Roles DbSet.
        /// </summary>
        DbSet<Role> Roles { get; }
        
        /// <summary>
        /// Gets the RefreshTokens DbSet.
        /// </summary>
        DbSet<RefreshToken> RefreshTokens { get; }
        
        /// <summary>
        /// Saves all changes made in this context to the database asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>The number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Returns a DbSet instance for access to entities of the given type in the context.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity for which a set should be returned.</typeparam>
        /// <returns>A set for the given entity type.</returns>
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        
        /// <summary>
        /// Gets an EntityEntry for the given entity. The entry provides access to change tracking information and operations for the entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity to get the entry for.</param>
        /// <returns>The entry for the entity.</returns>
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    }
}