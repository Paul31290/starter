using StarterTemplate.Application.DTOs;
using System.Net;
using System.Text.Json;

namespace StarterTemplate.Api.Middleware
{
    /// <summary>
    /// Global exception handling middleware for the API.
    /// Catches unhandled exceptions and returns appropriate error responses.
    /// </summary>
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new ApiResponseDto<object>
            {
                Success = false,
                Message = "An error occurred while processing your request.",
                Data = null
            };

            switch (exception)
            {
                case ArgumentException argEx:
                    response.Message = argEx.Message;
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;

                case UnauthorizedAccessException:
                    response.Message = "Unauthorized access.";
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    break;

                case KeyNotFoundException:
                    response.Message = "The requested resource was not found.";
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;

                case InvalidOperationException invalidOpEx:
                    response.Message = invalidOpEx.Message;
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;

                case TimeoutException:
                    response.Message = "The request timed out.";
                    context.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                    break;

                case NotImplementedException:
                    response.Message = "This feature is not implemented yet.";
                    context.Response.StatusCode = (int)HttpStatusCode.NotImplemented;
                    break;

                default:
                    response.Message = "An internal server error occurred.";
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            // Add exception details in development environment
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                response.Data = new
                {
                    Exception = exception.GetType().Name,
                    Message = exception.Message,
                    StackTrace = exception.StackTrace
                };
            }

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
