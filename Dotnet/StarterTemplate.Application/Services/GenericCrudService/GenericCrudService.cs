using StarterTemplate.Application.Interfaces;
using StarterTemplate.Application.DTOs;
using StarterTemplate.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace StarterTemplate.Application.Services.GenericCrudService
{
    /// <summary>
    /// Generic CRUD service implementation that provides common business operations for entities.
    /// This class implements the IGenericCrudService interface and handles entity-to-DTO mapping.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity this service handles.</typeparam>
    /// <typeparam name="TDto">The type of DTO used for data transfer.</typeparam>
    /// <typeparam name="TMapper">The type of mapper used for entity-DTO conversion.</typeparam>
    public class GenericCrudService<TEntity, TDto, TMapper> : IGenericCrudService<TEntity, TDto>
        where TEntity : BaseEntity
        where TDto : BaseDto
        where TMapper : class, IEntityMapper<TEntity, TDto>
    {
        /// <summary>
        /// The generic repository used for data access operations.
        /// </summary>
        protected readonly IGenericRepository<TEntity> _repository;

        /// <summary>
        /// The mapper used for converting between entities and DTOs.
        /// </summary>
        protected readonly TMapper _mapper;

        /// <summary>
        /// The HTTP context accessor used for accessing the current user information.
        /// </summary>
        protected readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the GenericCrudService class.
        /// </summary>
        /// <param name="repository">The generic repository to use for data access.</param>
        /// <param name="mapper">The mapper to use for entity-DTO conversion.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor to get current user information.</param>
        public GenericCrudService(
            IGenericRepository<TEntity> repository,
            TMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Gets the user ID of the currently authenticated user.
        /// </summary>
        /// <returns>The user ID if authenticated; otherwise, null.</returns>
        protected int? GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }
            return null;
        }

        /// <summary>
        /// Gets the username of the currently authenticated user.
        /// </summary>
        /// <returns>The username if authenticated; otherwise, null.</returns>
        protected string? GetCurrentUsername()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
        }

        /// <summary>
        /// Provides a function to include navigation properties in queries.
        /// Override this method in derived classes to include specific navigation properties.
        /// By default, includes audit trail navigation properties (CreatedBy, ModifiedBy).
        /// </summary>
        /// <returns>A function that modifies a query to include navigation properties.</returns>
        protected virtual Func<IQueryable<TEntity>, IQueryable<TEntity>> IncludeNavigationProperties()
            => query => query
                .Include(e => e.CreatedBy)
                .Include(e => e.ModifiedBy); // Includes audit trail navigation properties by default

        /// <summary>
        /// Retrieves all entities asynchronously and converts them to DTOs.
        /// </summary>
        /// <returns>A collection of DTOs representing all entities.</returns>
        public virtual async Task<IEnumerable<TDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync(include: IncludeNavigationProperties());
            return entities.Select(_mapper.ToDto);
        }

        /// <summary>
        /// Retrieves entities in a paginated format and converts them to DTOs.
        /// </summary>
        /// <param name="paginationRequest">The pagination parameters including page number, page size, and sorting options.</param>
        /// <returns>A paginated response containing DTOs and pagination metadata.</returns>
        public virtual async Task<PaginatedResponseDto<TDto>> GetPagedAsync(PaginationRequestDto paginationRequest)
        {
            var paginatedEntities = await _repository.GetPagedAsync(paginationRequest, include: IncludeNavigationProperties());

            return new PaginatedResponseDto<TDto>
            {
                Items = paginatedEntities.Items.Select(_mapper.ToDto),
                TotalCount = paginatedEntities.TotalCount,
                PageNumber = paginatedEntities.PageNumber,
                PageSize = paginatedEntities.PageSize,
                TotalPages = paginatedEntities.TotalPages,
                HasPrevious = paginatedEntities.HasPrevious,
                HasNext = paginatedEntities.HasNext
            };
        }

        /// <summary>
        /// Retrieves an entity by its unique identifier asynchronously and converts it to a DTO.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to retrieve.</param>
        /// <returns>The DTO if the entity is found; otherwise, null.</returns>
        public virtual async Task<TDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id, IncludeNavigationProperties());
            return entity == null ? default : _mapper.ToDto(entity);
        }

        /// <summary>
        /// Creates a new entity from the provided DTO asynchronously.
        /// </summary>
        /// <param name="dto">The DTO containing the data for the new entity.</param>
        /// <returns>The created entity converted to a DTO.</returns>
        public virtual async Task<TDto> CreateAsync(TDto dto)
        {
            var entity = _mapper.ToEntity(dto);

            // Set audit fields on the entity
            var currentUserId = GetCurrentUserId();
            entity.CreatedAt = DateTimeOffset.UtcNow;
            if (currentUserId.HasValue)
            {
                entity.CreatedById = currentUserId.Value;
            }

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            // Reload the entity with navigation properties to ensure we have the complete data
            var createdEntity = await _repository.GetByIdAsync(entity.Id, IncludeNavigationProperties());
            return _mapper.ToDto(createdEntity);
        }

        /// <summary>
        /// Updates an existing entity with the provided DTO asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to update.</param>
        /// <param name="dto">The DTO containing the updated data.</param>
        /// <returns>The updated entity converted to a DTO if found; otherwise, null.</returns>
        public virtual async Task<TDto?> UpdateAsync(int id, TDto dto)
        {
            var entity = await _repository.GetByIdAsync(id, IncludeNavigationProperties());
            if (entity == null) return default;

            _mapper.UpdateEntity(entity, dto); // now supported in mapper

            // Set audit fields on the entity
            var currentUserId = GetCurrentUserId();
            entity.ModifiedAt = DateTimeOffset.UtcNow;
            if (currentUserId.HasValue)
            {
                entity.ModifiedById = currentUserId.Value;
            }

            _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();

            // Reload the entity with navigation properties to ensure we have the latest data
            var updatedEntity = await _repository.GetByIdAsync(id, IncludeNavigationProperties());
            return updatedEntity == null ? default : _mapper.ToDto(updatedEntity);
        }

        /// <summary>
        /// Deletes an entity by its unique identifier asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete.</param>
        /// <returns>True if the entity was successfully deleted; otherwise, false.</returns>
        public virtual async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            _repository.Remove(entity);
            await _repository.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Exports entities to CSV format asynchronously with optional filtering and search.
        /// </summary>
        /// <param name="searchTerm">Optional search term to filter entities.</param>
        /// <param name="sortBy">Optional property name to sort by.</param>
        /// <param name="sortDirection">Optional sort direction ("asc" or "desc").</param>
        /// <returns>A CSV string containing the exported data.</returns>
        public virtual async Task<string> ExportToCsvAsync(string? searchTerm = null, string? sortBy = null, string? sortDirection = null)
        {
            // Create ordering function if sortBy is provided
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null;
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                orderBy = query => ApplySorting(query, sortBy, sortDirection);
            }

            return await _repository.ExportToCsvAsync(
                include: IncludeNavigationProperties(),
                orderBy: orderBy,
                searchTerm: searchTerm
            );
        }

        /// <summary>
        /// Gets the mapper instance for entity-DTO conversion.
        /// </summary>
        /// <returns>The mapper instance.</returns>
        public TMapper GetMapper()
        {
            return _mapper;
        }

        /// <summary>
        /// Applies sorting to the query based on the specified property and direction.
        /// </summary>
        /// <param name="query">The query to apply sorting to.</param>
        /// <param name="sortBy">The property name to sort by.</param>
        /// <param name="sortDirection">The sort direction ("asc" or "desc").</param>
        /// <returns>The modified query with sorting applied.</returns>
        private IOrderedQueryable<TEntity> ApplySorting(IQueryable<TEntity> query, string sortBy, string? sortDirection)
        {
            var parameter = System.Linq.Expressions.Expression.Parameter(typeof(TEntity), "x");
            var property = typeof(TEntity).GetProperty(sortBy, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            if (property == null)
                return query.OrderBy(x => x.Id); // Default sorting by Id

            var propertyExpression = System.Linq.Expressions.Expression.Property(parameter, property);
            var lambda = System.Linq.Expressions.Expression.Lambda(propertyExpression, parameter);

            var methodName = sortDirection?.ToLower() == "desc" ? "OrderByDescending" : "OrderBy";
            var method = typeof(Queryable).GetMethods()
                .Where(m => m.Name == methodName && m.GetParameters().Length == 2)
                .First()
                .MakeGenericMethod(typeof(TEntity), property.PropertyType);

            return (IOrderedQueryable<TEntity>)method.Invoke(null, new object[] { query, lambda });
        }
    }
}
