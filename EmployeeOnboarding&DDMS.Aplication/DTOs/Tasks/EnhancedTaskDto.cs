using System.Text.Json.Serialization;

namespace EmployeeOnboarding_DDMS.Aplication.DTOs.Tasks
{
    public class EnhancedTaskDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int TaskTemplateId { get; set; }
        public string TaskTemplateName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TaskDescription { get; set; } = string.Empty;
        public Domain.Enums.TaskStatus Status { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        
        // Enhanced properties
        public bool IsOverdue { get; set; }
        public int DaysUntilDue { get; set; }
        public int DaysOverdue { get; set; }
        public int Priority { get; set; }
        public bool RequiresDocumentUpload { get; set; }
        public string? HrComments { get; set; }
        public List<string> CompletionRequirements { get; set; } = new();
        public List<TaskDocumentDto> Documents { get; set; } = new();
        public bool CanUploadDocument { get; set; }
        public bool CanMarkComplete { get; set; }
        
        [JsonPropertyName("pdfTemplateUrl")]
        public string? PdfTemplateUrl { get; set; }
    }

    public class TaskDocumentDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public Domain.Enums.DocumentStatus Status { get; set; }
        public DateTime UploadDate { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewedBy { get; set; }
        public string? ReviewComments { get; set; }
        public int Version { get; set; }
    }
}
