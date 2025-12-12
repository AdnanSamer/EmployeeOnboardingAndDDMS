using EmployeeOnboarding_DDMS.Domain.Common;
using EmployeeOnboarding_DDMS.Domain.Enums;

namespace EmployeeOnboarding_DDMS.Domain.Entities
{
    public class Notification : AuditableBaseEntity
    {
        public int UserId { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Severity { get; set; } = "info"; // success, warning, danger, info
        public bool IsRead { get; set; } = false;
        public string? RelatedEntityType { get; set; }
        public int? RelatedEntityId { get; set; }
        public string? ActionUrl { get; set; }
        public DateTime? ReadDate { get; set; }

        // Navigation properties
        public virtual User User { get; set; } = null!;
    }
}

