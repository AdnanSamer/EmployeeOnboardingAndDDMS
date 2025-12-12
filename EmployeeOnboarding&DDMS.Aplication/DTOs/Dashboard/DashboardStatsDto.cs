namespace EmployeeOnboarding_DDMS.Aplication.DTOs.Dashboard
{
    public class DashboardStatsDto
    {
        public int TotalEmployees { get; set; }
        public int EmployeesInOnboarding { get; set; }
        public int CompletedOnboardings { get; set; }
        public int PendingTasks { get; set; }
        public int OverdueTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int TotalDocuments { get; set; }
        public int PendingDocumentReviews { get; set; }
    }
}

