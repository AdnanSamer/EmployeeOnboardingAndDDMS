using EmployeeOnboarding_DDMS.Domain.Common;

namespace EmployeeOnboarding_DDMS.Domain.Entities
{
    public class TaskTemplate : AuditableBaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsRequired { get; set; } = true;
        public bool RequiresDocumentUpload { get; set; } = false;
        public int? EstimatedDays { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<OnboardingTask> OnboardingTasks { get; set; } = new List<OnboardingTask>();
    }
}

