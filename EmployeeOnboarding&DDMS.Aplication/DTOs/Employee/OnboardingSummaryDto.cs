namespace EmployeeOnboarding_DDMS.Aplication.DTOs.Employee
{
    public class OnboardingSummaryDto
    {
        public EmployeeInfoDto Employee { get; set; } = new();
        public OnboardingProgressSummaryDto OnboardingProgress { get; set; } = new();
        public List<TaskSummaryDto> Tasks { get; set; } = new();
        public List<TimelineEventDto> Timeline { get; set; } = new();
    }

    public class EmployeeInfoDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public string Department { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
    }

    public class OnboardingProgressSummaryDto
    {
        public DateTime StartDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public int DaysElapsed { get; set; }
        public double ProgressPercentage { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class TaskSummaryDto
    {
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public List<DocumentSummaryItemDto> Documents { get; set; } = new();
    }

    public class DocumentSummaryItemDto
    {
        public string FileName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? ReviewComments { get; set; }
    }

    public class TimelineEventDto
    {
        public DateTime Date { get; set; }
        public string Event { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}
