using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarterTemplate.Infrastructure.Data;
using StarterTemplate.Api.Controllers;

namespace StarterTemplate.Api.Controllers
{
    /// <summary>
    /// Controller for health check endpoints.
    /// Provides system health status and monitoring information.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : BaseApiController
    {
        private readonly StarterTemplateContext _context;
        private readonly ILogger<HealthController> _logger;

        public HealthController(StarterTemplateContext context, ILogger<HealthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Basic health check endpoint.
        /// </summary>
        /// <returns>Health status information.</returns>
        [HttpGet]
        public ActionResult<object> GetHealth()
        {
            return Ok(new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0",
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
            });
        }

        /// <summary>
        /// Detailed health check with database connectivity.
        /// </summary>
        /// <returns>Detailed health status including database connectivity.</returns>
        [HttpGet("detailed")]
        public async Task<ActionResult<object>> GetDetailedHealth()
        {
            var healthStatus = new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0",
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                Checks = new
                {
                    Database = await CheckDatabaseHealth(),
                    Memory = GetMemoryHealth(),
                    Disk = GetDiskHealth()
                }
            };

            return Ok(healthStatus);
        }

        /// <summary>
        /// Database connectivity health check.
        /// </summary>
        /// <returns>Database health status.</returns>
        [HttpGet("database")]
        public async Task<ActionResult<object>> GetDatabaseHealth()
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();
                var connectionString = _context.Database.GetConnectionString();
                
                return Ok(new
                {
                    Status = canConnect ? "Healthy" : "Unhealthy",
                    CanConnect = canConnect,
                    ConnectionString = connectionString?.Substring(0, Math.Min(50, connectionString?.Length ?? 0)) + "...",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database health check failed");
                return StatusCode(500, new
                {
                    Status = "Unhealthy",
                    Error = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// System metrics health check.
        /// </summary>
        /// <returns>System metrics and performance information.</returns>
        [HttpGet("metrics")]
        public ActionResult<object> GetMetrics()
        {
            var process = System.Diagnostics.Process.GetCurrentProcess();
            var memory = GC.GetTotalMemory(false);

            return Ok(new
            {
                Timestamp = DateTime.UtcNow,
                Process = new
                {
                    Id = process.Id,
                    Name = process.ProcessName,
                    StartTime = process.StartTime,
                    WorkingSet = process.WorkingSet64,
                    VirtualMemorySize = process.VirtualMemorySize64,
                    Threads = process.Threads.Count
                },
                GarbageCollection = new
                {
                    TotalMemory = memory,
                    Gen0Collections = GC.CollectionCount(0),
                    Gen1Collections = GC.CollectionCount(1),
                    Gen2Collections = GC.CollectionCount(2)
                },
                System = new
                {
                    ProcessorCount = Environment.ProcessorCount,
                    MachineName = Environment.MachineName,
                    OSVersion = Environment.OSVersion.ToString(),
                    RuntimeVersion = Environment.Version.ToString()
                }
            });
        }

        private async Task<object> CheckDatabaseHealth()
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();
                var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
                var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync();

                return new
                {
                    Status = canConnect ? "Healthy" : "Unhealthy",
                    CanConnect = canConnect,
                    PendingMigrations = pendingMigrations.Count(),
                    AppliedMigrations = appliedMigrations.Count(),
                    Timestamp = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database health check failed");
                return new
                {
                    Status = "Unhealthy",
                    Error = ex.Message,
                    Timestamp = DateTime.UtcNow
                };
            }
        }

        private static object GetMemoryHealth()
        {
            var memory = GC.GetTotalMemory(false);
            var maxMemory = GC.GetTotalMemory(true);

            return new
            {
                TotalMemory = memory,
                MaxMemory = maxMemory,
                Gen0Collections = GC.CollectionCount(0),
                Gen1Collections = GC.CollectionCount(1),
                Gen2Collections = GC.CollectionCount(2),
                Timestamp = DateTime.UtcNow
            };
        }

        private static object GetDiskHealth()
        {
            try
            {
                var drives = DriveInfo.GetDrives()
                    .Where(d => d.IsReady)
                    .Select(d => new
                    {
                        Name = d.Name,
                        TotalSize = d.TotalSize,
                        AvailableSpace = d.AvailableFreeSpace,
                        UsedSpace = d.TotalSize - d.AvailableFreeSpace,
                        FreeSpacePercentage = (double)d.AvailableFreeSpace / d.TotalSize * 100
                    })
                    .ToList();

                return new
                {
                    Drives = drives,
                    Timestamp = DateTime.UtcNow
                };
            }
            catch (Exception)
            {
                return new
                {
                    Error = "Unable to retrieve disk information",
                    Timestamp = DateTime.UtcNow
                };
            }
        }
    }
}
