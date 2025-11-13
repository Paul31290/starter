using System.IO;
using System.Threading.Tasks;

namespace StarterTemplate.Application.Interfaces
{
    /// <summary>
    /// Interface for file storage operations.
    /// Provides methods for uploading, downloading, and managing files.
    /// </summary>
    public interface IFileStorageService
    {
        /// <summary>
        /// Uploads a file asynchronously.
        /// </summary>
        /// <param name="fileStream">The file stream to upload.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="folder">Optional folder path within the storage.</param>
        /// <returns>The URL or path of the uploaded file.</returns>
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string? folder = null);

        /// <summary>
        /// Downloads a file asynchronously.
        /// </summary>
        /// <param name="filePath">The path or URL of the file to download.</param>
        /// <returns>A stream containing the file data.</returns>
        Task<Stream> DownloadFileAsync(string filePath);

        /// <summary>
        /// Deletes a file asynchronously.
        /// </summary>
        /// <param name="filePath">The path or URL of the file to delete.</param>
        /// <returns>True if the file was deleted successfully; otherwise, false.</returns>
        Task<bool> DeleteFileAsync(string filePath);

        /// <summary>
        /// Checks if a file exists.
        /// </summary>
        /// <param name="filePath">The path or URL of the file to check.</param>
        /// <returns>True if the file exists; otherwise, false.</returns>
        Task<bool> FileExistsAsync(string filePath);

        /// <summary>
        /// Gets the file size in bytes.
        /// </summary>
        /// <param name="filePath">The path or URL of the file.</param>
        /// <returns>The file size in bytes.</returns>
        Task<long> GetFileSizeAsync(string filePath);

        /// <summary>
        /// Gets the MIME type of a file.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>The MIME type of the file.</returns>
        string GetMimeType(string fileName);

        /// <summary>
        /// Validates if a file type is allowed.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="allowedExtensions">Array of allowed file extensions.</param>
        /// <returns>True if the file type is allowed; otherwise, false.</returns>
        bool IsFileTypeAllowed(string fileName, string[] allowedExtensions);

        /// <summary>
        /// Validates if a file size is within the allowed limit.
        /// </summary>
        /// <param name="fileSize">The size of the file in bytes.</param>
        /// <param name="maxSizeInBytes">The maximum allowed file size in bytes.</param>
        /// <returns>True if the file size is within the limit; otherwise, false.</returns>
        bool IsFileSizeAllowed(long fileSize, long maxSizeInBytes);
    }
}
