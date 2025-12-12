namespace EmployeeOnboarding_DDMS.Aplication.DTOs.Dashboard
{
    public class EmployeeDashboardDto
    {
        public OnboardingProgressDto OnboardingProgress { get; set; } = new();
        public DocumentSummaryDto DocumentSummary { get; set; } = new();
        public List<ActivityDto> RecentActivity { get; set; } = new();
        public List<UpcomingDeadlineDto> UpcomingDeadlines { get; set; } = new();
    }

    public class OnboardingProgressDto
    {
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int OverdueTasks { get; set; }
        public double ProgressPercentage { get; set; }
        public DateTime? EstimatedCompletionDate { get; set; }
    }

    public class DocumentSummaryDto
    {
        public int TotalDocuments { get; set; }
        public int ApprovedDocuments { get; set; }
        public int PendingDocuments { get; set; }
        public int RejectedDocuments { get; set; }
    }

    public class ActivityDto
    {
        public string Type { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Severity { get; set; } = string.Empty;
        public string? Comments { get; set; }
    }

    public class UpcomingDeadlineDto
    {
        public int TaskId { get; set; }
        public string TaskTitle { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public int DaysRemaining { get; set; }
    }
}
