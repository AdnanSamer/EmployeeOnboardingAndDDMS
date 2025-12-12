using EmployeeOnboarding_DDMS.Domain.Common;
using EmployeeOnboarding_DDMS.Domain.Enums;

namespace EmployeeOnboarding_DDMS.Domain.Entities
{
    public class OnboardingTask : AuditableBaseEntity
    {
        public int EmployeeId { get; set; }
        public int TaskTemplateId { get; set; }
        public int AssignedBy { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime DueDate { get; set; }
        public Enums.TaskStatus Status { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string? Notes { get; set; }

        // Navigation properties
        public virtual Employee Employee { get; set; } = null!;
        public virtual TaskTemplate TaskTemplate { get; set; } = null!;
        public virtual User AssignedByUser { get; set; } = null!;
        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}

