using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using StarterTemplate.Application.DTOs;

namespace StarterTemplate.Application.Interfaces
{
    /// <summary>
    /// Generic repository interface that provides common data access operations for entities.
    /// This interface defines the contract for all repository implementations in the system.
    /// </summary>
    /// <typeparam name="T">The type of entity this repository handles.</typeparam>
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Retrieves all entities asynchronously with optional filtering, ordering, and inclusion of related entities.
        /// </summary>
        /// <param name="filter">Optional filter expression to apply to the query.</param>
        /// <param name="orderBy">Optional ordering function to apply to the query.</param>
        /// <param name="include">Optional function to include related entities in the query.</param>
        /// <returns>A collection of entities matching the specified criteria.</returns>
        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null
        );

        /// <summary>
        /// Retrieves entities in a paginated format with optional filtering, ordering, and inclusion of related entities.
        /// </summary>
        /// <param name="paginationRequest">The pagination parameters including page number, page size, and sorting options.</param>
        /// <param name="filter">Optional filter expression to apply to the query.</param>
        /// <param name="orderBy">Optional ordering function to apply to the query.</param>
        /// <param name="include">Optional function to include related entities in the query.</param>
        /// <returns>A paginated response containing the entities and pagination metadata.</returns>
        Task<PaginatedResponseDto<T>> GetPagedAsync(
            PaginationRequestDto paginationRequest,
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null
        );

        /// <summary>
        /// Retrieves an entity by its unique identifier asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to retrieve.</param>
        /// <param name="include">Optional function to include related entities in the query.</param>
        /// <returns>The entity if found; otherwise, null.</returns>
        Task<T?> GetByIdAsync(
            int id,
            Func<IQueryable<T>, IQueryable<T>>? include = null
        );

        /// <summary>
        /// Retrieves an entity by its unique identifier asynchronously with specified related entities included.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to retrieve.</param>
        /// <param name="includes">Array of expressions specifying which related entities to include.</param>
        /// <returns>The entity if found; otherwise, null.</returns>
        Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Finds entities that match the specified expression asynchronously.
        /// </summary>
        /// <param name="expression">The expression to filter entities by.</param>
        /// <returns>A collection of entities matching the expression.</returns>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression);

        /// <summary>
        /// Adds a new entity to the repository asynchronously.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The added entity with any generated values (such as ID).</returns>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Updates an existing entity in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>The updated entity.</returns>
        T UpdateAsync(T entity);

        /// <summary>
        /// Removes an entity from the repository.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        void Remove(T entity);

        /// <summary>
        /// Saves all changes made to entities in the repository asynchronously.
        /// </summary>
        /// <returns>The number of affected records.</returns>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Gets a queryable interface for the entity set.
        /// This allows for complex LINQ queries to be built.
        /// </summary>
        /// <returns>A queryable interface for the entity set.</returns>
        IQueryable<T> GetQueryable();

        /// <summary>
        /// Exports entities to CSV format asynchronously with optional filtering and ordering.
        /// </summary>
        /// <param name="filter">Optional filter expression to apply to the query.</param>
        /// <param name="orderBy">Optional ordering function to apply to the query.</param>
        /// <param name="include">Optional function to include related entities in the query.</param>
        /// <param name="searchTerm">Optional search term to filter entities.</param>
        /// <returns>A CSV string containing the exported data.</returns>
        Task<string> ExportToCsvAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null,
            string? searchTerm = null
        );
    }
}
