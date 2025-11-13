using System.ComponentModel.DataAnnotations;

namespace StarterTemplate.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for file upload operations.
    /// Used for handling file uploads with validation and metadata.
    /// </summary>
    public class FileUploadDto
    {
        /// <summary>
        /// Gets or sets the name of the uploaded file.
        /// </summary>
        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the MIME type of the file.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string ContentType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the size of the file in bytes.
        /// </summary>
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "File size must be greater than 0")]
        public long FileSize { get; set; }

        /// <summary>
        /// Gets or sets the file content as a base64 string.
        /// </summary>
        [Required]
        public string FileContent { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the folder path where the file should be stored.
        /// </summary>
        [StringLength(500)]
        public string? Folder { get; set; }

        /// <summary>
        /// Gets or sets additional metadata for the file.
        /// </summary>
        [StringLength(1000)]
        public string? Metadata { get; set; }
    }

    /// <summary>
    /// Data Transfer Object for file upload response.
    /// Contains information about the uploaded file.
    /// </summary>
    public class FileUploadResponseDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the uploaded file.
        /// </summary>
        public string FileId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the original file name.
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the URL or path to access the uploaded file.
        /// </summary>
        public string FileUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the size of the uploaded file in bytes.
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// Gets or sets the MIME type of the uploaded file.
        /// </summary>
        public string ContentType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time when the file was uploaded.
        /// </summary>
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the folder where the file was stored.
        /// </summary>
        public string? Folder { get; set; }
    }
}
