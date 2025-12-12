using EmployeeOnboarding_DDMS.Domain.Common;

namespace EmployeeOnboarding_DDMS.Domain.Entities
{
    public class ActivityLog : AuditableBaseEntity
    {
        public int? UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public int? EntityId { get; set; }
        public string? EntityName { get; set; }
        public string? Details { get; set; }
        public string? IpAddress { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

