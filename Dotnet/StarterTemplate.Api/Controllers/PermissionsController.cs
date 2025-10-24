using StarterTemplate.Api.Attributes;
using StarterTemplate.Application.DTOs;
using StarterTemplate.Application.Services;
using StarterTemplate.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace StarterTemplate.Api.Controllers
{
    /// <summary>
    /// Controller for managing permissions.
    /// Provides endpoints for permission CRUD operations.
    /// </summary>
    [RequirePermission("Permissions_List")]
    public class PermissionsController : BaseController<Permission, PermissionDto>
    {
        private readonly PermissionService _permissionService;

        /// <summary>
        /// Initializes a new instance of the PermissionsController class.
        /// </summary>
        /// <param name="service">The permission service for business operations.</param>
        public PermissionsController(PermissionService service) : base(service)
        {
            _permissionService = service;
        }

        /// <summary>
        /// Gets all permissions for a specific role.
        /// </summary>
        /// <param name="roleId">The ID of the role.</param>
        /// <returns>A collection of permissions assigned to the role.</returns>
        [HttpGet("role/{roleId}")]
        [RequirePermission("Permissions_List")]
        public async Task<ActionResult<IEnumerable<PermissionDto>>> GetPermissionsByRoleId(int roleId)
        {
            try
            {
                var permissions = await _permissionService.GetPermissionsByRoleIdAsync(roleId);
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving permissions.", error = ex.Message });
            }
        }

        /// <summary>
        /// Gets all permissions for a specific user (through their roles).
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A collection of permissions the user has through their roles.</returns>
        [HttpGet("user/{userId}")]
        [RequirePermission("Permissions_List")]
        public async Task<ActionResult<IEnumerable<PermissionDto>>> GetPermissionsByUserId(int userId)
        {
            try
            {
                var permissions = await _permissionService.GetPermissionsByUserIdAsync(userId);
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving permissions.", error = ex.Message });
            }
        }

        /// <summary>
        /// Checks if a user has a specific permission.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="permissionName">The name of the permission to check.</param>
        /// <returns>True if the user has the permission, otherwise false.</returns>
        [HttpGet("check/{userId}/{permissionName}")]
        [RequirePermission("Permissions_List")]
        public async Task<ActionResult<bool>> CheckUserPermission(int userId, string permissionName)
        {
            try
            {
                var hasPermission = await _permissionService.UserHasPermissionAsync(userId, permissionName);
                return Ok(hasPermission);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while checking permission.", error = ex.Message });
            }
        }

        /// <summary>
        /// Creates a new permission.
        /// </summary>
        [HttpPost]
        [RequirePermission("Permissions_Create")]
        public override Task<ActionResult<PermissionDto>> Create([FromBody] PermissionDto dto)
        {
            return base.Create(dto);
        }

        /// <summary>
        /// Updates an existing permission.
        /// </summary>
        [HttpPut("{id}")]
        [RequirePermission("Permissions_Update")]
        public override Task<ActionResult<PermissionDto>> Update(int id, [FromBody] PermissionDto dto)
        {
            return base.Update(id, dto);
        }

        /// <summary>
        /// Deletes a permission.
        /// </summary>
        [HttpDelete("{id}")]
        [RequirePermission("Permissions_Delete")]
        public override Task<ActionResult<bool>> Delete(int id)
        {
            return base.Delete(id);
        }

        /// <summary>
        /// Exports permissions to CSV.
        /// </summary>
        [HttpGet("export/csv")]
        [RequirePermission("Permissions_Export")]
        public override Task<IActionResult> ExportCsv([FromQuery] string? searchTerm = null, [FromQuery] string? sortBy = null, [FromQuery] string? sortDirection = null)
        {
            return base.ExportCsv(searchTerm, sortBy, sortDirection);
        }
    }
}

