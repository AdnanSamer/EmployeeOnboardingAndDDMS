using EmployeeOnboarding_DDMS.Domain.Common;
using EmployeeOnboarding_DDMS.Domain.Enums;

namespace EmployeeOnboarding_DDMS.Domain.Entities
{
    public class Document : AuditableBaseEntity
    {
        public int OnboardingTaskId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = "application/pdf";
        public int UploadedBy { get; set; }
        public DateTime UploadDate { get; set; }
        public int Version { get; set; } = 1;
        public int? ParentDocumentId { get; set; }
        public bool IsCurrentVersion { get; set; } = true;
        public string? Comments { get; set; }
        public DocumentStatus Status { get; set; }
        public int? ReviewedBy { get; set; }
        public DateTime? ReviewedDate { get; set; }
        public string? ReviewComments { get; set; }
        public string? AnnotationsJson { get; set; }
        public bool IsArchived { get; set; } = false;

        // Navigation properties
        public virtual OnboardingTask OnboardingTask { get; set; } = null!;
        public virtual User UploadedByUser { get; set; } = null!;
        public virtual User? ReviewedByUser { get; set; }
    }
}

