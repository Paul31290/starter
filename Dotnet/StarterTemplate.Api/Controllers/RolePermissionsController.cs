using StarterTemplate.Api.Attributes;
using StarterTemplate.Application.DTOs;
using StarterTemplate.Application.Services;
using StarterTemplate.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace StarterTemplate.Api.Controllers
{
    /// <summary>
    /// Controller for managing role-permission relationships.
    /// Provides endpoints for assigning and removing permissions from roles.
    /// </summary>
    [RequirePermission("RolePermissions_List")]
    public class RolePermissionsController : BaseController<RolePermission, RolePermissionDto>
    {
        private readonly RolePermissionService _rolePermissionService;

        /// <summary>
        /// Initializes a new instance of the RolePermissionsController class.
        /// </summary>
        /// <param name="service">The role-permission service for business operations.</param>
        public RolePermissionsController(RolePermissionService service) : base(service)
        {
            _rolePermissionService = service;
        }

        /// <summary>
        /// Gets all role permissions for a specific role.
        /// </summary>
        /// <param name="roleId">The ID of the role.</param>
        /// <returns>A collection of role-permission relationships.</returns>
        [HttpGet("role/{roleId}")]
        [RequirePermission("RolePermissions_List")]
        public async Task<ActionResult<IEnumerable<RolePermissionDto>>> GetRolePermissionsByRoleId(int roleId)
        {
            try
            {
                var rolePermissions = await _rolePermissionService.GetRolePermissionsByRoleIdAsync(roleId);
                return Ok(rolePermissions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving role permissions.", error = ex.Message });
            }
        }

        /// <summary>
        /// Assigns a permission to a role.
        /// </summary>
        /// <param name="roleId">The ID of the role.</param>
        /// <param name="permissionId">The ID of the permission.</param>
        /// <returns>The created role-permission relationship.</returns>
        [HttpPost("assign")]
        [RequirePermission("RolePermissions_Create")]
        public async Task<ActionResult<RolePermissionDto>> AssignPermissionToRole([FromQuery] int roleId, [FromQuery] int permissionId)
        {
            try
            {
                var rolePermission = await _rolePermissionService.AssignPermissionToRoleAsync(roleId, permissionId);
                return Ok(rolePermission);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while assigning permission.", error = ex.Message });
            }
        }

        /// <summary>
        /// Removes a permission from a role.
        /// </summary>
        /// <param name="roleId">The ID of the role.</param>
        /// <param name="permissionId">The ID of the permission.</param>
        /// <returns>True if successful, otherwise false.</returns>
        [HttpDelete("remove")]
        [RequirePermission("RolePermissions_Delete")]
        public async Task<ActionResult<bool>> RemovePermissionFromRole([FromQuery] int roleId, [FromQuery] int permissionId)
        {
            try
            {
                var result = await _rolePermissionService.RemovePermissionFromRoleAsync(roleId, permissionId);
                if (!result)
                {
                    return NotFound(new { message = "Role-permission relationship not found." });
                }
                return Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while removing permission.", error = ex.Message });
            }
        }

        /// <summary>
        /// Creates a new role-permission relationship.
        /// </summary>
        [HttpPost]
        [RequirePermission("RolePermissions_Create")]
        public override Task<ActionResult<RolePermissionDto>> Create([FromBody] RolePermissionDto dto)
        {
            return base.Create(dto);
        }

        /// <summary>
        /// Updates an existing role-permission relationship.
        /// </summary>
        [HttpPut("{id}")]
        [RequirePermission("RolePermissions_Update")]
        public override Task<ActionResult<RolePermissionDto>> Update(int id, [FromBody] RolePermissionDto dto)
        {
            return base.Update(id, dto);
        }

        /// <summary>
        /// Deletes a role-permission relationship.
        /// </summary>
        [HttpDelete("{id}")]
        [RequirePermission("RolePermissions_Delete")]
        public override Task<ActionResult<bool>> Delete(int id)
        {
            return base.Delete(id);
        }

        /// <summary>
        /// Exports role permissions to CSV.
        /// </summary>
        [HttpGet("export/csv")]
        [RequirePermission("RolePermissions_Export")]
        public override Task<IActionResult> ExportCsv([FromQuery] string? searchTerm = null, [FromQuery] string? sortBy = null, [FromQuery] string? sortDirection = null)
        {
            return base.ExportCsv(searchTerm, sortBy, sortDirection);
        }
    }
}

