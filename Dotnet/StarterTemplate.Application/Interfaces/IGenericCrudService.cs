using System.Collections.Generic;
using System.Threading.Tasks;
using StarterTemplate.Application.DTOs;

namespace StarterTemplate.Application.Interfaces
{
    /// <summary>
    /// Generic CRUD service interface that provides common business operations for entities.
    /// This interface defines the contract for all service implementations in the system.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity this service handles.</typeparam>
    /// <typeparam name="TDto">The type of DTO used for data transfer.</typeparam>
    public interface IGenericCrudService<TEntity, TDto>
        where TEntity : class
    {
        /// <summary>
        /// Retrieves all entities asynchronously and converts them to DTOs.
        /// </summary>
        /// <returns>A collection of DTOs representing all entities.</returns>
        Task<IEnumerable<TDto>> GetAllAsync();

        /// <summary>
        /// Retrieves entities in a paginated format and converts them to DTOs.
        /// </summary>
        /// <param name="paginationRequest">The pagination parameters including page number, page size, and sorting options.</param>
        /// <returns>A paginated response containing DTOs and pagination metadata.</returns>
        Task<PaginatedResponseDto<TDto>> GetPagedAsync(PaginationRequestDto paginationRequest);

        /// <summary>
        /// Retrieves an entity by its unique identifier asynchronously and converts it to a DTO.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to retrieve.</param>
        /// <returns>The DTO if the entity is found; otherwise, null.</returns>
        Task<TDto?> GetByIdAsync(int id);

        /// <summary>
        /// Creates a new entity from the provided DTO asynchronously.
        /// </summary>
        /// <param name="dto">The DTO containing the data for the new entity.</param>
        /// <returns>The created entity converted to a DTO.</returns>
        Task<TDto> CreateAsync(TDto dto);

        /// <summary>
        /// Updates an existing entity with the provided DTO asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to update.</param>
        /// <param name="dto">The DTO containing the updated data.</param>
        /// <returns>The updated entity converted to a DTO if found; otherwise, null.</returns>
        Task<TDto?> UpdateAsync(int id, TDto dto);

        /// <summary>
        /// Deletes an entity by its unique identifier asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete.</param>
        /// <returns>True if the entity was successfully deleted; otherwise, false.</returns>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Exports entities to CSV format asynchronously with optional filtering and search.
        /// </summary>
        /// <param name="searchTerm">Optional search term to filter entities.</param>
        /// <param name="sortBy">Optional property name to sort by.</param>
        /// <param name="sortDirection">Optional sort direction ("asc" or "desc").</param>
        /// <returns>A CSV string containing the exported data.</returns>
        Task<string> ExportToCsvAsync(string? searchTerm = null, string? sortBy = null, string? sortDirection = null);
    }
}