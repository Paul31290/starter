using StarterTemplate.Application.Interfaces;
using StarterTemplate.Application.DTOs;
using StarterTemplate.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace StarterTemplate.Application.Services
{
    /// <summary>
    /// Service for handling audit logging operations.
    /// Provides methods for tracking changes and user actions throughout the application.
    /// </summary>
    public class AuditService : IAuditService
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly ILogger<AuditService> _logger;

        public AuditService(IAuditLogRepository auditLogRepository, ILogger<AuditService> logger)
        {
            _auditLogRepository = auditLogRepository;
            _logger = logger;
        }

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
        public async Task<AuditLogDto> LogAsync(
            string entityName,
            string entityId,
            string action,
            string? changes = null,
            int? userId = null,
            string? ipAddress = null,
            string? userAgent = null)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    EntityName = entityName,
                    EntityId = entityId,
                    Action = action,
                    Changes = changes,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    Timestamp = DateTime.UtcNow,
                    UserId = userId,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedById = userId
                };

                var createdAuditLog = await _auditLogRepository.AddAsync(auditLog);
                await _auditLogRepository.SaveChangesAsync();

                _logger.LogInformation("Audit log created: {Action} on {EntityName} (ID: {EntityId}) by User {UserId}", 
                    action, entityName, entityId, userId);

                return new AuditLogDto
                {
                    Id = createdAuditLog.Id,
                    EntityName = createdAuditLog.EntityName,
                    EntityId = createdAuditLog.EntityId,
                    Action = createdAuditLog.Action,
                    Changes = createdAuditLog.Changes,
                    IpAddress = createdAuditLog.IpAddress,
                    UserAgent = createdAuditLog.UserAgent,
                    Timestamp = createdAuditLog.Timestamp,
                    UserId = createdAuditLog.UserId,
                    CreatedAt = createdAuditLog.CreatedAt,
                    CreatedById = createdAuditLog.CreatedById
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create audit log for {Action} on {EntityName} (ID: {EntityId})", 
                    action, entityName, entityId);
                throw;
            }
        }

        /// <summary>
        /// Logs a user login event asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user logging in.</param>
        /// <param name="ipAddress">Optional IP address of the user.</param>
        /// <param name="userAgent">Optional user agent string.</param>
        /// <param name="success">Whether the login was successful.</param>
        /// <param name="failureReason">Reason for login failure if applicable.</param>
        /// <returns>The created audit log entry.</returns>
        public async Task<AuditLogDto> LogLoginAsync(
            int userId,
            string? ipAddress = null,
            string? userAgent = null,
            bool success = true,
            string? failureReason = null)
        {
            var action = success ? "Login" : "LoginFailed";
            var changes = success ? null : JsonSerializer.Serialize(new { FailureReason = failureReason });

            return await LogAsync(
                entityName: "User",
                entityId: userId.ToString(),
                action: action,
                changes: changes,
                userId: userId,
                ipAddress: ipAddress,
                userAgent: userAgent);
        }

        /// <summary>
        /// Logs a user logout event asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user logging out.</param>
        /// <param name="ipAddress">Optional IP address of the user.</param>
        /// <param name="userAgent">Optional user agent string.</param>
        /// <returns>The created audit log entry.</returns>
        public async Task<AuditLogDto> LogLogoutAsync(
            int userId,
            string? ipAddress = null,
            string? userAgent = null)
        {
            return await LogAsync(
                entityName: "User",
                entityId: userId.ToString(),
                action: "Logout",
                userId: userId,
                ipAddress: ipAddress,
                userAgent: userAgent);
        }

        /// <summary>
        /// Logs a password change event asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user changing their password.</param>
        /// <param name="ipAddress">Optional IP address of the user.</param>
        /// <param name="userAgent">Optional user agent string.</param>
        /// <returns>The created audit log entry.</returns>
        public async Task<AuditLogDto> LogPasswordChangeAsync(
            int userId,
            string? ipAddress = null,
            string? userAgent = null)
        {
            return await LogAsync(
                entityName: "User",
                entityId: userId.ToString(),
                action: "PasswordChange",
                userId: userId,
                ipAddress: ipAddress,
                userAgent: userAgent);
        }

        /// <summary>
        /// Logs a permission change event asynchronously.
        /// </summary>
        /// <param name="targetUserId">The ID of the user whose permissions were changed.</param>
        /// <param name="performedByUserId">The ID of the user performing the change.</param>
        /// <param name="changes">JSON string describing the permission changes.</param>
        /// <param name="ipAddress">Optional IP address of the user.</param>
        /// <param name="userAgent">Optional user agent string.</param>
        /// <returns>The created audit log entry.</returns>
        public async Task<AuditLogDto> LogPermissionChangeAsync(
            int targetUserId,
            int performedByUserId,
            string changes,
            string? ipAddress = null,
            string? userAgent = null)
        {
            return await LogAsync(
                entityName: "User",
                entityId: targetUserId.ToString(),
                action: "PermissionChange",
                changes: changes,
                userId: performedByUserId,
                ipAddress: ipAddress,
                userAgent: userAgent);
        }

        /// <summary>
        /// Gets audit logs for a specific entity asynchronously.
        /// </summary>
        /// <param name="entityName">The name of the entity.</param>
        /// <param name="entityId">The ID of the entity.</param>
        /// <param name="pageNumber">Page number for pagination.</param>
        /// <param name="pageSize">Page size for pagination.</param>
        /// <returns>A paginated response containing audit logs.</returns>
        public async Task<PaginatedResponseDto<AuditLogDto>> GetEntityAuditLogsAsync(
            string entityName,
            string entityId,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                var auditLogs = await _auditLogRepository.GetByEntityAsync(entityName, entityId);
                var totalCount = auditLogs.Count();

                var pagedLogs = auditLogs
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(MapToDto)
                    .ToList();

                return new PaginatedResponseDto<AuditLogDto>
                {
                    Items = pagedLogs,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                    HasPrevious = pageNumber > 1,
                    HasNext = pageNumber < (int)Math.Ceiling((double)totalCount / pageSize)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get audit logs for entity {EntityName} (ID: {EntityId})", 
                    entityName, entityId);
                throw;
            }
        }

        /// <summary>
        /// Gets audit logs for a specific user asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="pageNumber">Page number for pagination.</param>
        /// <param name="pageSize">Page size for pagination.</param>
        /// <returns>A paginated response containing audit logs.</returns>
        public async Task<PaginatedResponseDto<AuditLogDto>> GetUserAuditLogsAsync(
            int userId,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                var auditLogs = await _auditLogRepository.GetByUserAsync(userId, pageNumber, pageSize);
                var totalCount = auditLogs.Count();

                var pagedLogs = auditLogs
                    .Select(MapToDto)
                    .ToList();

                return new PaginatedResponseDto<AuditLogDto>
                {
                    Items = pagedLogs,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                    HasPrevious = pageNumber > 1,
                    HasNext = pageNumber < (int)Math.Ceiling((double)totalCount / pageSize)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get audit logs for user {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Gets audit logs within a date range asynchronously.
        /// </summary>
        /// <param name="startDate">The start date for the range.</param>
        /// <param name="endDate">The end date for the range.</param>
        /// <param name="pageNumber">Page number for pagination.</param>
        /// <param name="pageSize">Page size for pagination.</param>
        /// <returns>A paginated response containing audit logs.</returns>
        public async Task<PaginatedResponseDto<AuditLogDto>> GetAuditLogsByDateRangeAsync(
            DateTime startDate,
            DateTime endDate,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                var auditLogs = await _auditLogRepository.GetByDateRangeAsync(startDate, endDate, pageNumber, pageSize);
                var totalCount = auditLogs.Count();

                var pagedLogs = auditLogs
                    .Select(MapToDto)
                    .ToList();

                return new PaginatedResponseDto<AuditLogDto>
                {
                    Items = pagedLogs,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                    HasPrevious = pageNumber > 1,
                    HasNext = pageNumber < (int)Math.Ceiling((double)totalCount / pageSize)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get audit logs for date range {StartDate} to {EndDate}", 
                    startDate, endDate);
                throw;
            }
        }

        /// <summary>
        /// Maps an AuditLog entity to an AuditLogDto.
        /// </summary>
        /// <param name="auditLog">The audit log entity.</param>
        /// <returns>The mapped audit log DTO.</returns>
        private static AuditLogDto MapToDto(AuditLog auditLog)
        {
            return new AuditLogDto
            {
                Id = auditLog.Id,
                EntityName = auditLog.EntityName,
                EntityId = auditLog.EntityId,
                Action = auditLog.Action,
                Changes = auditLog.Changes,
                IpAddress = auditLog.IpAddress,
                UserAgent = auditLog.UserAgent,
                Timestamp = auditLog.Timestamp,
                UserId = auditLog.UserId,
                CreatedAt = auditLog.CreatedAt,
                CreatedById = auditLog.CreatedById
            };
        }
    }
}
