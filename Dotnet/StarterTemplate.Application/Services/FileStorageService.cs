using StarterTemplate.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO;

namespace StarterTemplate.Application.Services
{
    /// <summary>
    /// Service for handling file storage operations.
    /// Provides methods for uploading, downloading, and managing files in local storage.
    /// </summary>
    public class FileStorageService : IFileStorageService
    {
        private readonly ILogger<FileStorageService> _logger;
        private readonly FileStorageSettings _fileStorageSettings;

        public FileStorageService(ILogger<FileStorageService> logger, IOptions<FileStorageSettings> fileStorageSettings)
        {
            _logger = logger;
            _fileStorageSettings = fileStorageSettings.Value;
        }

        /// <summary>
        /// Uploads a file asynchronously.
        /// </summary>
        /// <param name="fileStream">The file stream to upload.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="folder">Optional folder path within the storage.</param>
        /// <returns>The URL or path of the uploaded file.</returns>
        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string? folder = null)
        {
            try
            {
                if (!IsFileTypeAllowed(fileName, _fileStorageSettings.AllowedExtensions))
                {
                    throw new ArgumentException($"File type not allowed. Allowed extensions: {string.Join(", ", _fileStorageSettings.AllowedExtensions)}");
                }

                if (fileStream.Length > _fileStorageSettings.MaxFileSize)
                {
                    throw new ArgumentException($"File size exceeds maximum allowed size of {_fileStorageSettings.MaxFileSize} bytes");
                }

                var uploadPath = string.IsNullOrEmpty(folder)
                    ? _fileStorageSettings.LocalPath
                    : Path.Combine(_fileStorageSettings.LocalPath, folder);

                Directory.CreateDirectory(uploadPath);

                var fileExtension = Path.GetExtension(fileName);
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadPath, uniqueFileName);

                using var fileStreamOutput = new FileStream(filePath, FileMode.Create);
                await fileStream.CopyToAsync(fileStreamOutput);

                _logger.LogInformation("File uploaded successfully: {FileName} -> {FilePath}", fileName, filePath);

                return string.IsNullOrEmpty(folder)
                    ? $"uploads/{uniqueFileName}"
                    : $"uploads/{folder}/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload file: {FileName}", fileName);
                throw;
            }
        }

        /// <summary>
        /// Downloads a file asynchronously.
        /// </summary>
        /// <param name="filePath">The path or URL of the file to download.</param>
        /// <returns>A stream containing the file data.</returns>
        public async Task<Stream> DownloadFileAsync(string filePath)
        {
            try
            {
                var physicalPath = ConvertWebPathToPhysicalPath(filePath);

                if (!File.Exists(physicalPath))
                {
                    throw new FileNotFoundException($"File not found: {filePath}");
                }

                var fileStream = new FileStream(physicalPath, FileMode.Open, FileAccess.Read);
                _logger.LogInformation("File downloaded successfully: {FilePath}", filePath);

                return fileStream;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to download file: {FilePath}", filePath);
                throw;
            }
        }

        /// <summary>
        /// Deletes a file asynchronously.
        /// </summary>
        /// <param name="filePath">The path or URL of the file to delete.</param>
        /// <returns>True if the file was deleted successfully; otherwise, false.</returns>
        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                var physicalPath = ConvertWebPathToPhysicalPath(filePath);

                if (File.Exists(physicalPath))
                {
                    File.Delete(physicalPath);
                    _logger.LogInformation("File deleted successfully: {FilePath}", filePath);
                    return true;
                }

                _logger.LogWarning("File not found for deletion: {FilePath}", filePath);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete file: {FilePath}", filePath);
                return false;
            }
        }

        /// <summary>
        /// Checks if a file exists.
        /// </summary>
        /// <param name="filePath">The path or URL of the file to check.</param>
        /// <returns>True if the file exists; otherwise, false.</returns>
        public async Task<bool> FileExistsAsync(string filePath)
        {
            try
            {
                var physicalPath = ConvertWebPathToPhysicalPath(filePath);
                return File.Exists(physicalPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check file existence: {FilePath}", filePath);
                return false;
            }
        }

        /// <summary>
        /// Gets the file size in bytes.
        /// </summary>
        /// <param name="filePath">The path or URL of the file.</param>
        /// <returns>The file size in bytes.</returns>
        public async Task<long> GetFileSizeAsync(string filePath)
        {
            try
            {
                var physicalPath = ConvertWebPathToPhysicalPath(filePath);

                if (File.Exists(physicalPath))
                {
                    var fileInfo = new FileInfo(physicalPath);
                    return fileInfo.Length;
                }

                throw new FileNotFoundException($"File not found: {filePath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get file size: {FilePath}", filePath);
                throw;
            }
        }

        /// <summary>
        /// Gets the MIME type of a file.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>The MIME type of the file.</returns>
        public string GetMimeType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".txt" => "text/plain",
                ".csv" => "text/csv",
                ".zip" => "application/zip",
                _ => "application/octet-stream"
            };
        }

        /// <summary>
        /// Validates if a file type is allowed.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="allowedExtensions">Array of allowed file extensions.</param>
        /// <returns>True if the file type is allowed; otherwise, false.</returns>
        public bool IsFileTypeAllowed(string fileName, string[] allowedExtensions)
        {
            var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
            return allowedExtensions.Contains(fileExtension);
        }

        /// <summary>
        /// Validates if a file size is within the allowed limit.
        /// </summary>
        /// <param name="fileSize">The size of the file in bytes.</param>
        /// <param name="maxSizeInBytes">The maximum allowed file size in bytes.</param>
        /// <returns>True if the file size is within the limit; otherwise, false.</returns>
        public bool IsFileSizeAllowed(long fileSize, long maxSizeInBytes)
        {
            return fileSize <= maxSizeInBytes;
        }

        /// <summary>
        /// Converts a web path to a physical file system path.
        /// </summary>
        /// <param name="webPath">The web path (e.g., "uploads/file.jpg").</param>
        /// <returns>The physical file system path.</returns>
        private string ConvertWebPathToPhysicalPath(string webPath)
        {
            if (webPath.StartsWith("uploads/"))
            {
                webPath = webPath.Substring(8);
            }

            return Path.Combine(_fileStorageSettings.LocalPath, webPath);
        }
    }

    /// <summary>
    /// Configuration settings for file storage service.
    /// </summary>
    public class FileStorageSettings
    {
        public string Provider { get; set; } = "Local";
        public string LocalPath { get; set; } = "wwwroot/uploads";
        public long MaxFileSize { get; set; } = 10485760;
        public string[] AllowedExtensions { get; set; } = { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx", ".xls", ".xlsx" };
    }
}
