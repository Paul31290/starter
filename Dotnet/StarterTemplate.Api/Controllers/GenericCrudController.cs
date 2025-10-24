using Microsoft.AspNetCore.Mvc;
using StarterTemplate.Application.DTOs;
using StarterTemplate.Application.Interfaces;
using StarterTemplate.Domain.Entities;
using StarterTemplate.Application.Services.GenericCrudService;

namespace StarterTemplate.Api.Controllers
{
    /// <summary>
    /// Generic CRUD controller base class for entities.
    /// Provides standard CRUD operations for any entity type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TDto">The DTO type.</typeparam>
    [ApiController]
    [Route("api/[controller]")]
    public abstract class GenericCrudController<TEntity, TDto> : BaseApiController
        where TEntity : BaseEntity
        where TDto : BaseDto
    {
        protected readonly IGenericCrudService<TEntity, TDto> _crudService;

        protected GenericCrudController(IGenericCrudService<TEntity, TDto> crudService)
        {
            _crudService = crudService;
        }

        /// <summary>
        /// Gets all entities.
        /// </summary>
        /// <returns>A collection of entities.</returns>
        [HttpGet]
        public virtual async Task<ActionResult<IEnumerable<TDto>>> GetAll()
        {
            try
            {
                var result = await _crudService.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Gets entities with pagination.
        /// </summary>
        /// <param name="pageNumber">Page number for pagination.</param>
        /// <param name="pageSize">Page size for pagination.</param>
        /// <param name="searchTerm">Optional search term for filtering.</param>
        /// <param name="sortBy">Optional field to sort by.</param>
        /// <param name="sortDirection">Sort direction (asc or desc).</param>
        /// <returns>A paginated response containing the entities.</returns>
        [HttpGet("paged")]
        public virtual async Task<ActionResult<PaginatedResponseDto<TDto>>> GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string sortDirection = "asc")
        {
            try
            {
                var request = new PaginationRequestDto
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    SearchTerm = searchTerm,
                    SortBy = sortBy,
                    SortDirection = sortDirection
                };

                var result = await _crudService.GetPagedAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Gets an entity by its ID.
        /// </summary>
        /// <param name="id">The entity ID.</param>
        /// <returns>The entity if found.</returns>
        [HttpGet("{id}")]
        public virtual async Task<ActionResult<TDto>> GetById(int id)
        {
            try
            {
                var entity = await _crudService.GetByIdAsync(id);
                if (entity == null)
                {
                    return NotFound($"Entity with ID {id} not found.");
                }

                return Ok(entity);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Creates a new entity.
        /// </summary>
        /// <param name="dto">The entity data.</param>
        /// <returns>The created entity.</returns>
        [HttpPost]
        public virtual async Task<ActionResult<TDto>> Create([FromBody] TDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdEntity = await _crudService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = createdEntity.Id }, createdEntity);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="id">The entity ID.</param>
        /// <param name="dto">The updated entity data.</param>
        /// <returns>The updated entity.</returns>
        [HttpPut("{id}")]
        public virtual async Task<ActionResult<TDto>> Update(int id, [FromBody] TDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != dto.Id)
                {
                    return BadRequest("ID mismatch between URL and request body.");
                }

                var updatedEntity = await _crudService.UpdateAsync(id, dto);
                if (updatedEntity == null)
                {
                    return NotFound($"Entity with ID {id} not found.");
                }

                return Ok(updatedEntity);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Deletes an entity by its ID.
        /// </summary>
        /// <param name="id">The entity ID.</param>
        /// <returns>True if the entity was deleted; otherwise, false.</returns>
        [HttpDelete("{id}")]
        public virtual async Task<ActionResult<bool>> Delete(int id)
        {
            try
            {
                var deleted = await _crudService.DeleteAsync(id);
                if (!deleted)
                {
                    return NotFound($"Entity with ID {id} not found.");
                }

                return Ok(true);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Exports entities to CSV format.
        /// </summary>
        /// <param name="searchTerm">Optional search term for filtering.</param>
        /// <param name="sortBy">Optional field to sort by.</param>
        /// <param name="sortDirection">Sort direction (asc or desc).</param>
        /// <returns>CSV data as a string.</returns>
        [HttpGet("export")]
        public virtual async Task<ActionResult<string>> ExportToCsv(
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string sortDirection = "asc")
        {
            try
            {
                var csvData = await _crudService.ExportToCsvAsync(searchTerm, sortBy, sortDirection);
                return Ok(csvData);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
