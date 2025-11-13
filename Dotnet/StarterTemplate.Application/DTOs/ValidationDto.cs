using System.ComponentModel.DataAnnotations;

namespace StarterTemplate.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for validation results.
    /// Used to return validation errors and warnings.
    /// </summary>
    public class ValidationResultDto
    {
        /// <summary>
        /// Gets or sets a value indicating whether the validation was successful.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets the list of validation errors.
        /// </summary>
        public List<ValidationErrorDto> Errors { get; set; } = new List<ValidationErrorDto>();

        /// <summary>
        /// Gets or sets the list of validation warnings.
        /// </summary>
        public List<ValidationWarningDto> Warnings { get; set; } = new List<ValidationWarningDto>();

        /// <summary>
        /// Gets or sets additional validation metadata.
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Data Transfer Object for validation errors.
    /// Contains detailed information about validation failures.
    /// </summary>
    public class ValidationErrorDto
    {
        /// <summary>
        /// Gets or sets the name of the property that failed validation.
        /// </summary>
        public string PropertyName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the attempted value that failed validation.
        /// </summary>
        public object? AttemptedValue { get; set; }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        public string? ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets additional error details.
        /// </summary>
        public Dictionary<string, object> Details { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Data Transfer Object for validation warnings.
    /// Contains information about validation warnings.
    /// </summary>
    public class ValidationWarningDto
    {
        /// <summary>
        /// Gets or sets the name of the property that generated the warning.
        /// </summary>
        public string PropertyName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the warning message.
        /// </summary>
        public string WarningMessage { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the warning code.
        /// </summary>
        public string? WarningCode { get; set; }

        /// <summary>
        /// Gets or sets additional warning details.
        /// </summary>
        public Dictionary<string, object> Details { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Data Transfer Object for custom validation rules.
    /// Used to define custom validation logic.
    /// </summary>
    public class ValidationRuleDto
    {
        /// <summary>
        /// Gets or sets the name of the validation rule.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string RuleName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the property name to validate.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string PropertyName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the validation expression or rule.
        /// </summary>
        [Required]
        public string ValidationExpression { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the error message to display when validation fails.
        /// </summary>
        [Required]
        [StringLength(500)]
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the validation type (Regex, Custom, etc.).
        /// </summary>
        [Required]
        [StringLength(50)]
        public string ValidationType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether this rule is active.
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
