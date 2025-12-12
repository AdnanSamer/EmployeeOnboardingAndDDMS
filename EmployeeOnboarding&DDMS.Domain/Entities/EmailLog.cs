using EmployeeOnboarding_DDMS.Domain.Common;

namespace EmployeeOnboarding_DDMS.Domain.Entities
{
    public class EmailLog : BaseEntity
    {
        public string ToEmail { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string EmailType { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Sent, Failed, Pending
        public DateTime? SentDate { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}

