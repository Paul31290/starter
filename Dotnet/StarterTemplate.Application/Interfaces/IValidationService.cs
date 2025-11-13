using StarterTemplate.Application.DTOs;

namespace StarterTemplate.Application.Interfaces
{
    /// <summary>
    /// Interface for validation service operations.
    /// Provides methods for validating DTOs and custom validation rules.
    /// </summary>
    public interface IValidationService
    {
        /// <summary>
        /// Validates a DTO using data annotations and custom validation rules.
        /// </summary>
        /// <typeparam name="T">The type of DTO to validate.</typeparam>
        /// <param name="dto">The DTO to validate.</param>
        /// <returns>A validation result containing errors and warnings.</returns>
        Task<ValidationResultDto> ValidateAsync<T>(T dto) where T : class;

        /// <summary>
        /// Validates multiple DTOs in batch.
        /// </summary>
        /// <typeparam name="T">The type of DTOs to validate.</typeparam>
        /// <param name="dtos">The DTOs to validate.</param>
        /// <returns>A list of validation results for each DTO.</returns>
        Task<List<ValidationResultDto>> ValidateBatchAsync<T>(IEnumerable<T> dtos) where T : class;

        /// <summary>
        /// Validates a specific property of a DTO.
        /// </summary>
        /// <typeparam name="T">The type of DTO.</typeparam>
        /// <param name="dto">The DTO to validate.</param>
        /// <param name="propertyName">The name of the property to validate.</param>
        /// <returns>A validation result for the specific property.</returns>
        Task<ValidationResultDto> ValidatePropertyAsync<T>(T dto, string propertyName) where T : class;

        /// <summary>
        /// Validates a custom validation rule.
        /// </summary>
        /// <param name="rule">The validation rule to apply.</param>
        /// <param name="value">The value to validate.</param>
        /// <returns>True if the validation passes; otherwise, false.</returns>
        Task<bool> ValidateCustomRuleAsync(ValidationRuleDto rule, object? value);

        /// <summary>
        /// Gets validation rules for a specific type.
        /// </summary>
        /// <typeparam name="T">The type to get validation rules for.</typeparam>
        /// <returns>A list of validation rules.</returns>
        Task<List<ValidationRuleDto>> GetValidationRulesAsync<T>() where T : class;
    }
}
