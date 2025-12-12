using System.Text.Json.Serialization;

namespace EmployeeOnboarding_DDMS.Aplication.DTOs.Tasks
{
    public class CreateTaskTemplateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsRequired { get; set; } = true;
        
        [JsonPropertyName("requiresDocument")]
        public bool RequiresDocumentUpload { get; set; } = false;
        
        public int? EstimatedDays { get; set; }
    }
}

