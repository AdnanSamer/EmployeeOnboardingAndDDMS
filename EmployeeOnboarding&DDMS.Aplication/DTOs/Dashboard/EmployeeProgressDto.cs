using System.Text.Json.Serialization;

namespace EmployeeOnboarding_DDMS.Aplication.DTOs.Dashboard
{
    public class EmployeeProgressDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Department { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
        public int OverdueTasks { get; set; }
        public double ProgressPercentage { get; set; }
        
        [JsonPropertyName("completionPercentage")]
        public int CompletionPercentage { get; set; }
        
        public DateTime? OnboardingCompletedDate { get; set; }
    }
}

