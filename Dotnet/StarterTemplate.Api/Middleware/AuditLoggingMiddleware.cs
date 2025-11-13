using StarterTemplate.Application.Interfaces;
using System.Security.Claims;

namespace StarterTemplate.Api.Middleware
{
    /// <summary>
    /// Middleware for automatic audit logging of API requests.
    /// Logs user actions and API calls for audit trail purposes.
    /// </summary>
    public class AuditLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditLoggingMiddleware> _logger;

        public AuditLoggingMiddleware(RequestDelegate next, ILogger<AuditLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IAuditService auditService)
        {
            // Skip audit logging for certain paths
            if (ShouldSkipAuditLogging(context.Request.Path))
            {
                await _next(context);
                return;
            }

            var startTime = DateTime.UtcNow;
            var userId = GetUserId(context);
            var ipAddress = GetClientIpAddress(context);
            var userAgent = context.Request.Headers["User-Agent"].FirstOrDefault();

            try
            {
                await _next(context);
            }
            finally
            {
                // Log the request after processing
                await LogRequestAsync(auditService, context, userId, ipAddress, userAgent, startTime);
            }
        }

        private static bool ShouldSkipAuditLogging(PathString path)
        {
            var skipPaths = new[]
            {
                "/swagger",
                "/health",
                "/favicon.ico",
                "/_framework",
                "/css",
                "/js",
                "/images"
            };

            return skipPaths.Any(skipPath => path.StartsWithSegments(skipPath));
        }

        private static int? GetUserId(HttpContext context)
        {
            var userIdClaim = context.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }
            return null;
        }

        private static string? GetClientIpAddress(HttpContext context)
        {
            // Check for forwarded IP first (for load balancers/proxies)
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }

            var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                return realIp;
            }

            return context.Connection.RemoteIpAddress?.ToString();
        }

        private async Task LogRequestAsync(
            IAuditService auditService,
            HttpContext context,
            int? userId,
            string? ipAddress,
            string? userAgent,
            DateTime startTime)
        {
            try
            {
                var endTime = DateTime.UtcNow;
                var duration = endTime - startTime;

                var action = GetActionFromRequest(context);
                var entityName = GetEntityNameFromPath(context.Request.Path);
                var entityId = GetEntityIdFromPath(context.Request.Path);

                var changes = new
                {
                    Method = context.Request.Method,
                    Path = context.Request.Path.Value,
                    QueryString = context.Request.QueryString.Value,
                    StatusCode = context.Response.StatusCode,
                    Duration = duration.TotalMilliseconds,
                    ContentType = context.Request.ContentType,
                    ContentLength = context.Request.ContentLength
                };

                await auditService.LogAsync(
                    entityName: entityName,
                    entityId: entityId,
                    action: action,
                    changes: System.Text.Json.JsonSerializer.Serialize(changes),
                    userId: userId,
                    ipAddress: ipAddress,
                    userAgent: userAgent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log audit information for request: {Path}", context.Request.Path);
                // Don't rethrow - audit logging failure shouldn't break the request
            }
        }

        private static string GetActionFromRequest(HttpContext context)
        {
            return context.Request.Method.ToUpperInvariant() switch
            {
                "GET" => "Read",
                "POST" => "Create",
                "PUT" => "Update",
                "PATCH" => "Update",
                "DELETE" => "Delete",
                _ => "Unknown"
            };
        }

        private static string GetEntityNameFromPath(PathString path)
        {
            var segments = path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments != null && segments.Length >= 2)
            {
                // Remove "api" prefix and version if present
                var entitySegment = segments[1];
                if (entitySegment.StartsWith("v") && segments.Length > 2)
                {
                    entitySegment = segments[2];
                }
                return entitySegment;
            }
            return "Unknown";
        }

        private static string GetEntityIdFromPath(PathString path)
        {
            var segments = path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments != null && segments.Length >= 3)
            {
                // Look for numeric ID in the path
                for (int i = 2; i < segments.Length; i++)
                {
                    if (int.TryParse(segments[i], out _))
                    {
                        return segments[i];
                    }
                }
            }
            return "N/A";
        }
    }
}
