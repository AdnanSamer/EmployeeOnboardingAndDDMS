using System.Text.Json.Serialization;

namespace EmployeeOnboarding_DDMS.Aplication.DTOs.Tasks
{
    public class TaskTemplateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsRequired { get; set; }
        
        [JsonPropertyName("requiresDocument")]
        public bool RequiresDocumentUpload { get; set; }
        
        public int? EstimatedDays { get; set; }
        public bool IsActive { get; set; }
    }
}

