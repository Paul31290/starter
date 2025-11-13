using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace StarterTemplate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VersionController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;
            
            return Ok(new
            {
                version = version?.ToString() ?? "1.0.0",
                assemblyName = assembly.GetName().Name,
                buildDate = GetBuildDate(assembly),
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"
            });
        }

        private static DateTime GetBuildDate(Assembly assembly)
        {
            try
            {
                var buildDateAttribute = assembly.GetCustomAttribute<AssemblyMetadataAttribute>();
                if (buildDateAttribute != null && buildDateAttribute.Key == "BuildDate")
                {
                    if (DateTime.TryParse(buildDateAttribute.Value, out var buildDate))
                    {
                        return buildDate;
                    }
                }
            }
            catch
            {
                // Ignore errors and return current date
            }

            // Fallback to file creation date
            try
            {
                var fileInfo = new FileInfo(assembly.Location);
                return fileInfo.CreationTime;
            }
            catch
            {
                // Final fallback
                return DateTime.Now;
            }
        }
    }
} 