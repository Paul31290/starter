using System;
using System.Threading.Tasks;
using StarterTemplate.Application.DTOs;

namespace StarterTemplate.Application.Interfaces
{
    /// <summary>
    /// Interface for audit logging operations.
    /// Provides methods for tracking changes and user actions.
    /// </summary>
    public interface IAuditService
    {
        /// <summary>
        /// Logs an audit entry asynchronously.
        /// </summary>
        /// <param name="entityName">The name of the entity being audited.</param>
        /// <param name="entityId">The ID of the entity being audited.</param>
        /// <param name="action">The action performed (Create, Update, Delete, etc.).</param>
        /// <param name="changes">Optional JSON string describing the changes made.</param>
        /// <param name="userId">The ID of the user performing the action.</param>
        /// <param name="ipAddress">Optional IP address of the user.</param>
        /// <param name="userAgent">Optional user agent string.</param>
        /// <returns>The created audit log entry.</returns>
        Task<AuditLogDto> LogAsync(
            string entityName,
            string entityId,
            string action,
            string? changes = null,
            int? userId = null,
            string? ipAddress = null,
            string? userAgent = null);

        /// <summary>
        /// Logs a user login event asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user logging in.</param>
        /// <param name="ipAddress">Optional IP address of the user.</param>
        /// <param name="userAgent">Optional user agent string.</param>
        /// <param name="success">Whether the login was successful.</param>
        /// <param name="failureReason">Reason for login failure if applicable.</param>
        /// <returns>The created audit log entry.</returns>
        Task<AuditLogDto> LogLoginAsync(
            int userId,
            string? ipAddress = null,
            string? userAgent = null,
            bool success = true,
            string? failureReason = null);

        /// <summary>
        /// Logs a user logout event asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user logging out.</param>
        /// <param name="ipAddress">Optional IP address of the user.</param>
        /// <param name="userAgent">Optional user agent string.</param>
        /// <returns>The created audit log entry.</returns>
        Task<AuditLogDto> LogLogoutAsync(
            int userId,
            string? ipAddress = null,
            string? userAgent = null);

        /// <summary>
        /// Logs a password change event asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user changing their password.</param>
        /// <param name="ipAddress">Optional IP address of the user.</param>
        /// <param name="userAgent">Optional user agent string.</param>
        /// <returns>The created audit log entry.</returns>
        Task<AuditLogDto> LogPasswordChangeAsync(
            int userId,
            string? ipAddress = null,
            string? userAgent = null);

        /// <summary>
        /// Logs a permission change event asynchronously.
        /// </summary>
        /// <param name="targetUserId">The ID of the user whose permissions were changed.</param>
        /// <param name="performedByUserId">The ID of the user performing the change.</param>
        /// <param name="changes">JSON string describing the permission changes.</param>
        /// <param name="ipAddress">Optional IP address of the user.</param>
        /// <param name="userAgent">Optional user agent string.</param>
        /// <returns>The created audit log entry.</returns>
        Task<AuditLogDto> LogPermissionChangeAsync(
            int targetUserId,
            int performedByUserId,
            string changes,
            string? ipAddress = null,
            string? userAgent = null);

        /// <summary>
        /// Gets audit logs for a specific entity asynchronously.
        /// </summary>
        /// <param name="entityName">The name of the entity.</param>
        /// <param name="entityId">The ID of the entity.</param>
        /// <param name="pageNumber">Page number for pagination.</param>
        /// <param name="pageSize">Page size for pagination.</param>
        /// <returns>A paginated response containing audit logs.</returns>
        Task<PaginatedResponseDto<AuditLogDto>> GetEntityAuditLogsAsync(
            string entityName,
            string entityId,
            int pageNumber = 1,
            int pageSize = 10);

        /// <summary>
        /// Gets audit logs for a specific user asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="pageNumber">Page number for pagination.</param>
        /// <param name="pageSize">Page size for pagination.</param>
        /// <returns>A paginated response containing audit logs.</returns>
        Task<PaginatedResponseDto<AuditLogDto>> GetUserAuditLogsAsync(
            int userId,
            int pageNumber = 1,
            int pageSize = 10);

        /// <summary>
        /// Gets audit logs within a date range asynchronously.
        /// </summary>
        /// <param name="startDate">The start date for the range.</param>
        /// <param name="endDate">The end date for the range.</param>
        /// <param name="pageNumber">Page number for pagination.</param>
        /// <param name="pageSize">Page size for pagination.</param>
        /// <returns>A paginated response containing audit logs.</returns>
        Task<PaginatedResponseDto<AuditLogDto>> GetAuditLogsByDateRangeAsync(
            DateTime startDate,
            DateTime endDate,
            int pageNumber = 1,
            int pageSize = 10);
    }
}
