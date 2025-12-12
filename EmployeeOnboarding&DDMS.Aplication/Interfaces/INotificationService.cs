using EmployeeOnboarding_DDMS.Aplication.DTOs.Notifications;
using EmployeeOnboarding_DDMS.Aplication.Wrappers;

namespace EmployeeOnboarding_DDMS.Aplication.Interfaces
{
    public interface INotificationService
    {
        Task<Response<IEnumerable<NotificationDto>>> GetEmployeeNotificationsAsync(int userId, bool unreadOnly = false);
        Task<Response<int>> GetUnreadCountAsync(int userId);
        Task<Response<bool>> MarkAsReadAsync(int notificationId, int userId);
        Task<Response<int>> MarkAllAsReadAsync(int userId);
        Task<Response<NotificationDto>> CreateNotificationAsync(int userId, string type, string title, string message, string severity, string? relatedEntityType = null, int? relatedEntityId = null, string? actionUrl = null);
    }
}
