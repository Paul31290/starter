using StarterTemplate.Api.Attributes;
using StarterTemplate.Application.DTOs;
using StarterTemplate.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StarterTemplate.Api.Controllers
{
    /// <summary>
    /// Base controller that provides common CRUD operations for entities.
    /// This abstract class implements standard HTTP endpoints for Create, Read, Update, and Delete operations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity this controller handles.</typeparam>
    /// <typeparam name="TDto">The type of DTO used for data transfer.</typeparam>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public abstract class BaseController<TEntity, TDto> : ControllerBase
        where TEntity : class
        where TDto : class
    {
        /// <summary>
        /// The generic CRUD service used for business operations.
        /// </summary>
        protected readonly IGenericCrudService<TEntity, TDto> _service;

        /// <summary>
        /// Initializes a new instance of the BaseController class.
        /// </summary>
        /// <param name="service">The generic CRUD service to use for operations.</param>
        protected BaseController(IGenericCrudService<TEntity, TDto> service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves all entities asynchronously.
        /// </summary>
        /// <returns>An action result containing a collection of DTOs.</returns>
        /// <response code="200">Returns the collection of entities.</response>
        /// <response code="500">If an error occurred while retrieving the entities.</response>
        [HttpGet]
        public virtual async Task<ActionResult<IEnumerable<TDto>>> GetAll()
        {
            try
            {
                var items = await _service.GetAllAsync();
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving items.", error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves entities in a paginated format with optional sorting and searching.
        /// </summary>
        /// <param name="pageNumber">The page number (default: 1).</param>
        /// <param name="pageSize">The number of items per page (default: 10).</param>
        /// <param name="sortBy">The property name to sort by.</param>
        /// <param name="sortDirection">The sort direction ("asc" or "desc", default: "asc").</param>
        /// <param name="searchTerm">The search term to filter entities.</param>
        /// <returns>An action result containing a paginated response of DTOs.</returns>
        /// <response code="200">Returns the paginated collection of entities.</response>
        /// <response code="500">If an error occurred while retrieving the entities.</response>
        [HttpGet("paged")]
        public virtual async Task<ActionResult<PaginatedResponseDto<TDto>>> GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortDirection = "asc",
            [FromQuery] string? searchTerm = null)
        {
            try
            {
                var paginationRequest = new PaginationRequestDto
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    SortBy = sortBy,
                    SortDirection = sortDirection,
                    SearchTerm = searchTerm
                };

                var result = await _service.GetPagedAsync(paginationRequest);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving items.", error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to retrieve.</param>
        /// <returns>An action result containing the DTO if found.</returns>
        /// <response code="200">Returns the requested entity.</response>
        /// <response code="404">If the entity was not found.</response>
        /// <response code="500">If an error occurred while retrieving the entity.</response>
        [HttpGet("{id}")]
        public virtual async Task<ActionResult<TDto>> GetById(int id)
        {
            try
            {
                var item = await _service.GetByIdAsync(id);
                if (item == null)
                {
                    return NotFound(new { message = "Item not found." });
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the item.", error = ex.Message });
            }
        }

        /// <summary>
        /// Creates a new entity from the provided DTO.
        /// </summary>
        /// <param name="dto">The DTO containing the data for the new entity.</param>
        /// <returns>An action result containing the created DTO.</returns>
        /// <response code="201">Returns the newly created entity.</response>
        /// <response code="400">If the DTO is invalid.</response>
        /// <response code="500">If an error occurred while creating the entity.</response>
        [HttpPost]
        public virtual async Task<ActionResult<TDto>> Create([FromBody] TDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdItem = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = GetIdFromDto(createdItem) }, createdItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the item.", error = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing entity with the provided DTO.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to update.</param>
        /// <param name="dto">The DTO containing the updated data.</param>
        /// <returns>An action result containing the updated DTO.</returns>
        /// <response code="200">Returns the updated entity.</response>
        /// <response code="400">If the DTO is invalid.</response>
        /// <response code="404">If the entity was not found.</response>
        /// <response code="500">If an error occurred while updating the entity.</response>
        [HttpPut("{id}")]
        public virtual async Task<ActionResult<TDto>> Update(int id, [FromBody] TDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedItem = await _service.UpdateAsync(id, dto);
                if (updatedItem == null)
                {
                    return NotFound(new { message = "Item not found." });
                }

                return Ok(updatedItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the item.", error = ex.Message });
            }
        }

        /// <summary>
        /// Deletes an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete.</param>
        /// <returns>An action result indicating the success of the deletion.</returns>
        /// <response code="200">If the entity was successfully deleted.</response>
        /// <response code="404">If the entity was not found.</response>
        /// <response code="500">If an error occurred while deleting the entity.</response>
        [HttpDelete("{id}")]
        public virtual async Task<ActionResult<bool>> Delete(int id)
        {
            try
            {
                var result = await _service.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Item not found." });
                }

                return Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the item.", error = ex.Message });
            }
        }

        /// <summary>
        /// Exports entities to CSV format with optional filtering and search.
        /// </summary>
        /// <param name="searchTerm">Optional search term to filter entities.</param>
        /// <param name="sortBy">Optional property name to sort by.</param>
        /// <param name="sortDirection">Optional sort direction ("asc" or "desc").</param>
        /// <returns>A CSV file containing the exported data.</returns>
        /// <response code="200">Returns the CSV file.</response>
        /// <response code="500">If an error occurred while exporting the data.</response>
        [HttpGet("export/csv")]
        public virtual async Task<IActionResult> ExportCsv(
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortDirection = null)
        {
            try
            {
                var csvData = await _service.ExportToCsvAsync(searchTerm, sortBy, sortDirection);

                if (string.IsNullOrEmpty(csvData))
                {
                    return NotFound(new { message = "No data available for export." });
                }

                var fileName = $"{typeof(TEntity).Name.ToLower()}_export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                var bytes = System.Text.Encoding.UTF8.GetBytes(csvData);

                return File(bytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while exporting the data.", error = ex.Message });
            }
        }

        /// <summary>
        /// Extracts the ID from a DTO object.
        /// This is a basic implementation that looks for an "Id" property.
        /// Override in derived controllers if needed for different ID property names.
        /// </summary>
        /// <param name="dto">The DTO object to extract the ID from.</param>
        /// <returns>The ID value if found; otherwise, 0.</returns>
        protected virtual int? GetIdFromDto(TDto dto)
        {
            // This is a basic implementation. Override in derived controllers if needed
            var idProperty = typeof(TDto).GetProperty("Id");
            if (idProperty != null)
            {
                var value = idProperty.GetValue(dto);
                // If the Id is boxed int (covers int and nullable int with value)
                if (value is int intValue)
                {
                    return intValue;
                }

                // If the Id is a string, try parse
                if (value is string s && int.TryParse(s, out var parsedInt))
                {
                    return parsedInt;
                }
            }
            return null;
        }
    }
}