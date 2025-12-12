using EmployeeOnboarding_DDMS.Domain.Enums;
using System.Text.Json.Serialization;

namespace EmployeeOnboarding_DDMS.Aplication.DTOs.Tasks
{
    public class TaskDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int TaskTemplateId { get; set; }
        public string TaskTemplateName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? TaskDescription { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime DueDate { get; set; }
        public Domain.Enums.TaskStatus Status { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int AssignedBy { get; set; }
        public string AssignedByName { get; set; } = string.Empty;
        public int Priority { get; set; }
        public string? Notes { get; set; }
        public bool RequiresDocumentUpload { get; set; }
        public int DocumentCount { get; set; }
        
        [JsonPropertyName("pdfTemplateUrl")]
        public string? PdfTemplateUrl { get; set; }
    }
}

