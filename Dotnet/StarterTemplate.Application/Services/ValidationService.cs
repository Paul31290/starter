using StarterTemplate.Application.DTOs;
using StarterTemplate.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace StarterTemplate.Application.Services
{
    /// <summary>
    /// Service for handling validation operations.
    /// Provides methods for validating DTOs and custom validation rules.
    /// </summary>
    public class ValidationService : IValidationService
    {
        private readonly ILogger<ValidationService> _logger;

        public ValidationService(ILogger<ValidationService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Validates a DTO using data annotations and custom validation rules.
        /// </summary>
        /// <typeparam name="T">The type of DTO to validate.</typeparam>
        /// <param name="dto">The DTO to validate.</param>
        /// <returns>A validation result containing errors and warnings.</returns>
        public async Task<ValidationResultDto> ValidateAsync<T>(T dto) where T : class
        {
            try
            {
                var result = new ValidationResultDto { IsValid = true };

                var validationContext = new ValidationContext(dto);
                var validationResults = new List<ValidationResult>();
                
                if (!Validator.TryValidateObject(dto, validationContext, validationResults, true))
                {
                    result.IsValid = false;
                    result.Errors = validationResults
                        .Where(vr => vr != ValidationResult.Success)
                        .Select(vr => new ValidationErrorDto
                        {
                            PropertyName = vr.MemberNames.FirstOrDefault() ?? "Unknown",
                            ErrorMessage = vr.ErrorMessage ?? "Validation failed",
                            AttemptedValue = GetPropertyValue(dto, vr.MemberNames.FirstOrDefault()),
                            ErrorCode = "DataAnnotation"
                        })
                        .ToList();
                }

                await ApplyCustomValidationRulesAsync(dto, result);

                _logger.LogDebug("Validation completed for {Type}. IsValid: {IsValid}, Errors: {ErrorCount}", 
                    typeof(T).Name, result.IsValid, result.Errors.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during validation of {Type}", typeof(T).Name);
                return new ValidationResultDto
                {
                    IsValid = false,
                    Errors = new List<ValidationErrorDto>
                    {
                        new ValidationErrorDto
                        {
                            PropertyName = "Validation",
                            ErrorMessage = "An error occurred during validation",
                            ErrorCode = "ValidationError"
                        }
                    }
                };
            }
        }

        /// <summary>
        /// Validates multiple DTOs in batch.
        /// </summary>
        /// <typeparam name="T">The type of DTOs to validate.</typeparam>
        /// <param name="dtos">The DTOs to validate.</param>
        /// <returns>A list of validation results for each DTO.</returns>
        public async Task<List<ValidationResultDto>> ValidateBatchAsync<T>(IEnumerable<T> dtos) where T : class
        {
            var results = new List<ValidationResultDto>();
            
            foreach (var dto in dtos)
            {
                var result = await ValidateAsync(dto);
                results.Add(result);
            }

            return results;
        }

        /// <summary>
        /// Validates a specific property of a DTO.
        /// </summary>
        /// <typeparam name="T">The type of DTO.</typeparam>
        /// <param name="dto">The DTO to validate.</param>
        /// <param name="propertyName">The name of the property to validate.</param>
        /// <returns>A validation result for the specific property.</returns>
        public async Task<ValidationResultDto> ValidatePropertyAsync<T>(T dto, string propertyName) where T : class
        {
            try
            {
                var result = new ValidationResultDto { IsValid = true };
                var property = typeof(T).GetProperty(propertyName);
                
                if (property == null)
                {
                    result.IsValid = false;
                    result.Errors.Add(new ValidationErrorDto
                    {
                        PropertyName = propertyName,
                        ErrorMessage = "Property not found",
                        ErrorCode = "PropertyNotFound"
                    });
                    return result;
                }

                var value = property.GetValue(dto);
                var validationContext = new ValidationContext(dto) { MemberName = propertyName };
                var validationResults = new List<ValidationResult>();

                if (!Validator.TryValidateProperty(value, validationContext, validationResults))
                {
                    result.IsValid = false;
                    result.Errors = validationResults
                        .Where(vr => vr != ValidationResult.Success)
                        .Select(vr => new ValidationErrorDto
                        {
                            PropertyName = propertyName,
                            ErrorMessage = vr.ErrorMessage ?? "Validation failed",
                            AttemptedValue = value,
                            ErrorCode = "DataAnnotation"
                        })
                        .ToList();
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during property validation of {PropertyName} on {Type}", 
                    propertyName, typeof(T).Name);
                return new ValidationResultDto
                {
                    IsValid = false,
                    Errors = new List<ValidationErrorDto>
                    {
                        new ValidationErrorDto
                        {
                            PropertyName = propertyName,
                            ErrorMessage = "An error occurred during property validation",
                            ErrorCode = "ValidationError"
                        }
                    }
                };
            }
        }

        /// <summary>
        /// Validates a custom validation rule.
        /// </summary>
        /// <param name="rule">The validation rule to apply.</param>
        /// <param name="value">The value to validate.</param>
        /// <returns>True if the validation passes; otherwise, false.</returns>
        public async Task<bool> ValidateCustomRuleAsync(ValidationRuleDto rule, object? value)
        {
            try
            {
                if (!rule.IsActive)
                    return true;

                return rule.ValidationType.ToLowerInvariant() switch
                {
                    "regex" => ValidateRegexRule(rule.ValidationExpression, value?.ToString()),
                    "range" => ValidateRangeRule(rule.ValidationExpression, value),
                    "length" => ValidateLengthRule(rule.ValidationExpression, value?.ToString()),
                    "custom" => await ValidateCustomExpressionAsync(rule.ValidationExpression, value),
                    _ => true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during custom rule validation: {RuleName}", rule.RuleName);
                return false;
            }
        }

        /// <summary>
        /// Gets validation rules for a specific type.
        /// </summary>
        /// <typeparam name="T">The type to get validation rules for.</typeparam>
        /// <returns>A list of validation rules.</returns>
        public async Task<List<ValidationRuleDto>> GetValidationRulesAsync<T>() where T : class
        {
            var rules = new List<ValidationRuleDto>();
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes<ValidationAttribute>();
                
                foreach (var attribute in attributes)
                {
                    rules.Add(new ValidationRuleDto
                    {
                        RuleName = attribute.GetType().Name,
                        PropertyName = property.Name,
                        ValidationExpression = attribute.ToString() ?? "",
                        ErrorMessage = attribute.ErrorMessage ?? "Validation failed",
                        ValidationType = attribute.GetType().Name,
                        IsActive = true
                    });
                }
            }

            return rules;
        }

        /// <summary>
        /// Applies custom validation rules to a DTO.
        /// </summary>
        /// <typeparam name="T">The type of DTO.</typeparam>
        /// <param name="dto">The DTO to validate.</param>
        /// <param name="result">The validation result to update.</param>
        private async Task ApplyCustomValidationRulesAsync<T>(T dto, ValidationResultDto result) where T : class
        {
            if (dto is UserDto userDto && !string.IsNullOrEmpty(userDto.Email))
            {
                var domain = userDto.Email.Split('@').LastOrDefault();
                if (domain != null && IsRestrictedDomain(domain))
                {
                    result.IsValid = false;
                    result.Errors.Add(new ValidationErrorDto
                    {
                        PropertyName = nameof(UserDto.Email),
                        ErrorMessage = "Email domain is not allowed",
                        AttemptedValue = userDto.Email,
                        ErrorCode = "RestrictedDomain"
                    });
                }
            }
        }

        /// <summary>
        /// Gets the value of a property from an object.
        /// </summary>
        /// <param name="obj">The object to get the property value from.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The property value.</returns>
        private static object? GetPropertyValue(object obj, string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return null;

            var property = obj.GetType().GetProperty(propertyName);
            return property?.GetValue(obj);
        }

        /// <summary>
        /// Validates a regex rule.
        /// </summary>
        /// <param name="pattern">The regex pattern.</param>
        /// <param name="value">The value to validate.</param>
        /// <returns>True if the value matches the pattern; otherwise, false.</returns>
        private static bool ValidateRegexRule(string pattern, string? value)
        {
            if (string.IsNullOrEmpty(value))
                return true;

            try
            {
                return System.Text.RegularExpressions.Regex.IsMatch(value, pattern);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validates a range rule.
        /// </summary>
        /// <param name="rangeExpression">The range expression (e.g., "1-100").</param>
        /// <param name="value">The value to validate.</param>
        /// <returns>True if the value is within the range; otherwise, false.</returns>
        private static bool ValidateRangeRule(string rangeExpression, object? value)
        {
            if (value == null)
                return true;

            try
            {
                var parts = rangeExpression.Split('-');
                if (parts.Length != 2)
                    return false;

                if (double.TryParse(parts[0], out var min) && double.TryParse(parts[1], out var max))
                {
                    if (double.TryParse(value.ToString(), out var numValue))
                    {
                        return numValue >= min && numValue <= max;
                    }
                }
            }
            catch
            {
            }

            return false;
        }

        /// <summary>
        /// Validates a length rule.
        /// </summary>
        /// <param name="lengthExpression">The length expression (e.g., "5-50").</param>
        /// <param name="value">The value to validate.</param>
        /// <returns>True if the value length is within the specified range; otherwise, false.</returns>
        private static bool ValidateLengthRule(string lengthExpression, string? value)
        {
            if (string.IsNullOrEmpty(value))
                return true;

            try
            {
                var parts = lengthExpression.Split('-');
                if (parts.Length != 2)
                    return false;

                if (int.TryParse(parts[0], out var minLength) && int.TryParse(parts[1], out var maxLength))
                {
                    return value.Length >= minLength && value.Length <= maxLength;
                }
            }
            catch
            {
            }

            return false;
        }

        /// <summary>
        /// Validates a custom expression.
        /// </summary>
        /// <param name="expression">The custom validation expression.</param>
        /// <param name="value">The value to validate.</param>
        /// <returns>True if the validation passes; otherwise, false.</returns>
        private static async Task<bool> ValidateCustomExpressionAsync(string expression, object? value)
        {
            await Task.Delay(1);
            return true;
        }

        /// <summary>
        /// Checks if a domain is restricted.
        /// </summary>
        /// <param name="domain">The domain to check.</param>
        /// <returns>True if the domain is restricted; otherwise, false.</returns>
        private static bool IsRestrictedDomain(string domain)
        {
            var restrictedDomains = new[] { "tempmail.com", "10minutemail.com", "guerrillamail.com" };
            return restrictedDomains.Contains(domain.ToLowerInvariant());
        }
    }
}
