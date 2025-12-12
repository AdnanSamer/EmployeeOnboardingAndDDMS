using EmployeeOnboarding_DDMS.Domain.Enums;

namespace EmployeeOnboarding_DDMS.Aplication.DTOs.Documents
{
    public class DocumentDto
    {
        public int Id { get; set; }
        public int OnboardingTaskId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public int UploadedBy { get; set; }
        public string UploadedByName { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; }
        public int Version { get; set; }
        public bool IsCurrentVersion { get; set; }
        public string? Comments { get; set; }
        public DocumentStatus Status { get; set; }
        public int? ReviewedBy { get; set; }
        public string? ReviewedByName { get; set; }
        public DateTime? ReviewedDate { get; set; }
        public string? ReviewComments { get; set; }
    }
}

