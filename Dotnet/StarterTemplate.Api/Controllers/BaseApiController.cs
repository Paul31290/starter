using StarterTemplate.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace StarterTemplate.Api.Controllers
{
    /// <summary>
    /// Base API controller that provides common functionality for all controllers.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        /// <summary>
        /// Handles exceptions and returns appropriate error responses.
        /// </summary>
        /// <param name="ex">The exception to handle.</param>
        /// <returns>An appropriate error response.</returns>
        protected ActionResult HandleException(Exception ex)
        {
            var response = new ApiResponseDto<object>
            {
                Success = false,
                Message = ex.Message,
                Data = null
            };

            return ex switch
            {
                ArgumentException => BadRequest(response),
                UnauthorizedAccessException => Unauthorized(response),
                KeyNotFoundException => NotFound(response),
                InvalidOperationException => BadRequest(response),
                TimeoutException => StatusCode(408, response),
                NotImplementedException => StatusCode(501, response),
                _ => StatusCode(500, response)
            };
        }

        /// <summary>
        /// Creates a successful API response.
        /// </summary>
        /// <typeparam name="T">The type of data.</typeparam>
        /// <param name="data">The data to return.</param>
        /// <param name="message">Optional message.</param>
        /// <returns>A successful API response.</returns>
        protected ActionResult<ApiResponseDto<T>> Success<T>(T data, string? message = null)
        {
            return Ok(new ApiResponseDto<T>
            {
                Success = true,
                Message = message ?? "Operation completed successfully",
                Data = data
            });
        }

        /// <summary>
        /// Creates an error API response.
        /// </summary>
        /// <typeparam name="T">The type of data.</typeparam>
        /// <param name="message">The error message.</param>
        /// <param name="data">Optional data.</param>
        /// <returns>An error API response.</returns>
        protected ActionResult<ApiResponseDto<T>> Error<T>(string message, T? data = default)
        {
            return Ok(new ApiResponseDto<T>
            {
                Success = false,
                Message = message,
                Data = data
            });
        }
    }
}
