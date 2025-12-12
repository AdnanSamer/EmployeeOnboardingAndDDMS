using EmployeeOnboarding_DDMS.Aplication.DTOs.Notifications;
using EmployeeOnboarding_DDMS.Aplication.Interfaces;
using EmployeeOnboarding_DDMS.Aplication.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeOnboarding_DDMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Get employee notifications
        /// </summary>
        [HttpGet("employee")]
        public async Task<ActionResult<Response<IEnumerable<NotificationDto>>>> GetEmployeeNotifications([FromQuery] bool unreadOnly = false)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            if (userId == 0)
            {
                return Unauthorized(new Response<IEnumerable<NotificationDto>>("Invalid token."));
            }

            var result = await _notificationService.GetEmployeeNotificationsAsync(userId, unreadOnly);
            return Ok(result);
        }

        /// <summary>
        /// Get unread notification count
        /// </summary>
        [HttpGet("unread-count")]
        public async Task<ActionResult<Response<int>>> GetUnreadCount()
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            if (userId == 0)
            {
                return Unauthorized(new Response<int>("Invalid token."));
            }

            var result = await _notificationService.GetUnreadCountAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Mark notification as read
        /// </summary>
        [HttpPut("{id}/mark-read")]
        public async Task<ActionResult<Response<bool>>> MarkAsRead(int id)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            if (userId == 0)
            {
                return Unauthorized(new Response<bool>("Invalid token."));
            }

            var result = await _notificationService.MarkAsReadAsync(id, userId);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Mark all notifications as read
        /// </summary>
        [HttpPut("mark-all-read")]
        public async Task<ActionResult<Response<int>>> MarkAllAsRead()
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            if (userId == 0)
            {
                return Unauthorized(new Response<int>("Invalid token."));
            }

            var result = await _notificationService.MarkAllAsReadAsync(userId);
            return Ok(result);
        }
    }
}
