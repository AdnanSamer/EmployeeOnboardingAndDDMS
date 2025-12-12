using AutoMapper;
using EmployeeOnboarding_DDMS.Aplication.DTOs.Notifications;
using EmployeeOnboarding_DDMS.Aplication.Interfaces;
using EmployeeOnboarding_DDMS.Aplication.Wrappers;
using EmployeeOnboarding_DDMS.Domain.Entities;
using EmployeeOnboarding_DDMS.Domain.Enums;
using EmployeeOnboarding_DDMS.Domain.Interfaces;

namespace EmployeeOnboarding_DDMS.Aplication.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;

        public NotificationService(INotificationRepository notificationRepository, IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        public async Task<Response<IEnumerable<NotificationDto>>> GetEmployeeNotificationsAsync(int userId, bool unreadOnly = false)
        {
            var notifications = await _notificationRepository.GetByUserIdAsync(userId, unreadOnly);
            var notificationDtos = _mapper.Map<IEnumerable<NotificationDto>>(notifications);
            return new Response<IEnumerable<NotificationDto>>(notificationDtos);
        }

        public async Task<Response<int>> GetUnreadCountAsync(int userId)
        {
            var count = await _notificationRepository.GetUnreadCountAsync(userId);
            return new Response<int>(count);
        }

        public async Task<Response<bool>> MarkAsReadAsync(int notificationId, int userId)
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationId);
            if (notification == null)
            {
                return new Response<bool>("Notification not found.");
            }

            if (notification.UserId != userId)
            {
                return new Response<bool>("Unauthorized access to notification.");
            }

            await _notificationRepository.MarkAsReadAsync(notificationId);
            return new Response<bool>(true, "Notification marked as read.");
        }

        public async Task<Response<int>> MarkAllAsReadAsync(int userId)
        {
            var unreadCount = await _notificationRepository.GetUnreadCountAsync(userId);
            await _notificationRepository.MarkAllAsReadAsync(userId);
            return new Response<int>(unreadCount, $"{unreadCount} notifications marked as read.");
        }

        public async Task<Response<NotificationDto>> CreateNotificationAsync(
            int userId,
            string type,
            string title,
            string message,
            string severity,
            string? relatedEntityType = null,
            int? relatedEntityId = null,
            string? actionUrl = null)
        {
            var notification = new Notification
            {
                UserId = userId,
                Type = Enum.Parse<NotificationType>(type),
                Title = title,
                Message = message,
                Severity = severity,
                RelatedEntityType = relatedEntityType,
                RelatedEntityId = relatedEntityId,
                ActionUrl = actionUrl,
                IsRead = false
            };

            var createdNotification = await _notificationRepository.AddAsync(notification);
            var notificationDto = _mapper.Map<NotificationDto>(createdNotification);
            return new Response<NotificationDto>(notificationDto, "Notification created successfully.");
        }
    }
}
