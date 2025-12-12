using EmployeeOnboarding_DDMS.Domain.Entities;

namespace EmployeeOnboarding_DDMS.Domain.Interfaces
{
    public interface INotificationRepository
    {
        // CRUD Operations
        Task<Notification?> GetByIdAsync(int id);
        Task<IReadOnlyList<Notification>> GetAllAsync();
        Task<Notification> AddAsync(Notification entity);
        Task UpdateAsync(Notification entity);
        Task DeleteAsync(Notification entity);

        // Custom Operations
        Task<IEnumerable<Notification>> GetByUserIdAsync(int userId, bool unreadOnly = false);
        Task<int> GetUnreadCountAsync(int userId);
        Task MarkAsReadAsync(int notificationId);
        Task MarkAllAsReadAsync(int userId);
    }
}
