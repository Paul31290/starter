using Microsoft.AspNetCore.Mvc;
using StarterTemplate.Application.DTOs;
using StarterTemplate.Application.Interfaces;
using StarterTemplate.Api.Controllers;

namespace StarterTemplate.Api.Controllers
{
    /// <summary>
    /// Controller for managing audit logs.
    /// Provides endpoints for retrieving audit trail information.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuditLogController : BaseApiController
    {
        private readonly IAuditService _auditService;

        public AuditLogController(IAuditService auditService)
        {
            _auditService = auditService;
        }

        /// <summary>
        /// Gets audit logs for a specific entity.
        /// </summary>
        /// <param name="entityName">The name of the entity.</param>
        /// <param name="entityId">The ID of the entity.</param>
        /// <param name="pageNumber">Page number for pagination.</param>
        /// <param name="pageSize">Page size for pagination.</param>
        /// <returns>A paginated response containing audit logs.</returns>
        [HttpGet("entity/{entityName}/{entityId}")]
        public async Task<ActionResult<PaginatedResponseDto<AuditLogDto>>> GetEntityAuditLogs(
            string entityName, 
            string entityId, 
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var auditLogs = await _auditService.GetEntityAuditLogsAsync(entityName, entityId, pageNumber, pageSize);
                return Ok(auditLogs);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Gets audit logs for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="pageNumber">Page number for pagination.</param>
        /// <param name="pageSize">Page size for pagination.</param>
        /// <returns>A paginated response containing audit logs.</returns>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<PaginatedResponseDto<AuditLogDto>>> GetUserAuditLogs(
            int userId, 
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var auditLogs = await _auditService.GetUserAuditLogsAsync(userId, pageNumber, pageSize);
                return Ok(auditLogs);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Gets audit logs within a date range.
        /// </summary>
        /// <param name="startDate">The start date for the range.</param>
        /// <param name="endDate">The end date for the range.</param>
        /// <param name="pageNumber">Page number for pagination.</param>
        /// <param name="pageSize">Page size for pagination.</param>
        /// <returns>A paginated response containing audit logs.</returns>
        [HttpGet("date-range")]
        public async Task<ActionResult<PaginatedResponseDto<AuditLogDto>>> GetAuditLogsByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate, 
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var auditLogs = await _auditService.GetAuditLogsByDateRangeAsync(startDate, endDate, pageNumber, pageSize);
                return Ok(auditLogs);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Logs a user login event.
        /// </summary>
        /// <param name="request">The login audit request.</param>
        /// <returns>The created audit log entry.</returns>
        [HttpPost("login")]
        public async Task<ActionResult<AuditLogDto>> LogLogin([FromBody] LoginAuditRequestDto request)
        {
            try
            {
                var auditLog = await _auditService.LogLoginAsync(
                    request.UserId, 
                    request.IpAddress, 
                    request.UserAgent, 
                    request.Success, 
                    request.FailureReason);
                
                return Ok(auditLog);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Logs a user logout event.
        /// </summary>
        /// <param name="request">The logout audit request.</param>
        /// <returns>The created audit log entry.</returns>
        [HttpPost("logout")]
        public async Task<ActionResult<AuditLogDto>> LogLogout([FromBody] LogoutAuditRequestDto request)
        {
            try
            {
                var auditLog = await _auditService.LogLogoutAsync(
                    request.UserId, 
                    request.IpAddress, 
                    request.UserAgent);
                
                return Ok(auditLog);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Logs a password change event.
        /// </summary>
        /// <param name="request">The password change audit request.</param>
        /// <returns>The created audit log entry.</returns>
        [HttpPost("password-change")]
        public async Task<ActionResult<AuditLogDto>> LogPasswordChange([FromBody] PasswordChangeAuditRequestDto request)
        {
            try
            {
                var auditLog = await _auditService.LogPasswordChangeAsync(
                    request.UserId, 
                    request.IpAddress, 
                    request.UserAgent);
                
                return Ok(auditLog);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Logs a permission change event.
        /// </summary>
        /// <param name="request">The permission change audit request.</param>
        /// <returns>The created audit log entry.</returns>
        [HttpPost("permission-change")]
        public async Task<ActionResult<AuditLogDto>> LogPermissionChange([FromBody] PermissionChangeAuditRequestDto request)
        {
            try
            {
                var auditLog = await _auditService.LogPermissionChangeAsync(
                    request.TargetUserId, 
                    request.PerformedByUserId, 
                    request.Changes, 
                    request.IpAddress, 
                    request.UserAgent);
                
                return Ok(auditLog);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }

    /// <summary>
    /// Data Transfer Object for login audit requests.
    /// </summary>
    public class LoginAuditRequestDto
    {
        /// <summary>
        /// Gets or sets the ID of the user logging in.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the IP address of the user.
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the user agent string.
        /// </summary>
        public string? UserAgent { get; set; }

        /// <summary>
        /// Gets or sets whether the login was successful.
        /// </summary>
        public bool Success { get; set; } = true;

        /// <summary>
        /// Gets or sets the reason for login failure if applicable.
        /// </summary>
        public string? FailureReason { get; set; }
    }

    /// <summary>
    /// Data Transfer Object for logout audit requests.
    /// </summary>
    public class LogoutAuditRequestDto
    {
        /// <summary>
        /// Gets or sets the ID of the user logging out.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the IP address of the user.
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the user agent string.
        /// </summary>
        public string? UserAgent { get; set; }
    }

    /// <summary>
    /// Data Transfer Object for password change audit requests.
    /// </summary>
    public class PasswordChangeAuditRequestDto
    {
        /// <summary>
        /// Gets or sets the ID of the user changing their password.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the IP address of the user.
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the user agent string.
        /// </summary>
        public string? UserAgent { get; set; }
    }

    /// <summary>
    /// Data Transfer Object for permission change audit requests.
    /// </summary>
    public class PermissionChangeAuditRequestDto
    {
        /// <summary>
        /// Gets or sets the ID of the user whose permissions were changed.
        /// </summary>
        public int TargetUserId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user performing the change.
        /// </summary>
        public int PerformedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the JSON string describing the permission changes.
        /// </summary>
        public string Changes { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the IP address of the user.
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the user agent string.
        /// </summary>
        public string? UserAgent { get; set; }
    }
}
