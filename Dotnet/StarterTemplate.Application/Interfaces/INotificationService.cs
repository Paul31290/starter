using StarterTemplate.Application.DTOs;

namespace StarterTemplate.Application.Interfaces
{
    /// <summary>
    /// Interface for notification service operations.
    /// Provides methods for managing user notifications.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Creates a new notification for a user.
        /// </summary>
        /// <param name="userId">The ID of the user to notify.</param>
        /// <param name="title">The notification title.</param>
        /// <param name="message">The notification message.</param>
        /// <param name="severity">The notification severity level.</param>
        /// <returns>The created notification.</returns>
        Task<NotificationDto> CreateNotificationAsync(int userId, string title, string message, string severity = "Info");

        /// <summary>
        /// Creates notifications for multiple users.
        /// </summary>
        /// <param name="userIds">The IDs of the users to notify.</param>
        /// <param name="title">The notification title.</param>
        /// <param name="message">The notification message.</param>
        /// <param name="severity">The notification severity level.</param>
        /// <returns>The created notifications.</returns>
        Task<IEnumerable<NotificationDto>> CreateBulkNotificationsAsync(IEnumerable<int> userIds, string title, string message, string severity = "Info");

        /// <summary>
        /// Marks a notification as read.
        /// </summary>
        /// <param name="notificationId">The ID of the notification.</param>
        /// <param name="userId">The ID of the user marking the notification as read.</param>
        /// <returns>True if the notification was marked as read; otherwise, false.</returns>
        Task<bool> MarkAsReadAsync(int notificationId, int userId);

        /// <summary>
        /// Marks all notifications for a user as read.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The number of notifications marked as read.</returns>
        Task<int> MarkAllAsReadAsync(int userId);

        /// <summary>
        /// Gets unread notifications for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="pageNumber">Page number for pagination.</param>
        /// <param name="pageSize">Page size for pagination.</param>
        /// <returns>A paginated response containing unread notifications.</returns>
        Task<PaginatedResponseDto<NotificationDto>> GetUnreadNotificationsAsync(int userId, int pageNumber = 1, int pageSize = 10);

        /// <summary>
        /// Gets all notifications for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="pageNumber">Page number for pagination.</param>
        /// <param name="pageSize">Page size for pagination.</param>
        /// <returns>A paginated response containing all notifications.</returns>
        Task<PaginatedResponseDto<NotificationDto>> GetUserNotificationsAsync(int userId, int pageNumber = 1, int pageSize = 10);

        /// <summary>
        /// Deletes a notification.
        /// </summary>
        /// <param name="notificationId">The ID of the notification.</param>
        /// <param name="userId">The ID of the user deleting the notification.</param>
        /// <returns>True if the notification was deleted; otherwise, false.</returns>
        Task<bool> DeleteNotificationAsync(int notificationId, int userId);

        /// <summary>
        /// Deletes all read notifications for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The number of notifications deleted.</returns>
        Task<int> DeleteReadNotificationsAsync(int userId);

        /// <summary>
        /// Gets the count of unread notifications for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The count of unread notifications.</returns>
        Task<int> GetUnreadCountAsync(int userId);
    }
}
