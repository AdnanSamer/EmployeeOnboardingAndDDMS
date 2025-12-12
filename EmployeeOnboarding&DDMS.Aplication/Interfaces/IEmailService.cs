namespace EmployeeOnboarding_DDMS.Aplication.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
        Task SendWelcomeEmailAsync(string employeeEmail, string employeeName, string defaultPassword);
        Task SendTaskAssignedEmailAsync(string employeeEmail, string employeeName, string taskName, DateTime dueDate);
        Task SendTaskAssignedEmailAsync(string employeeEmail, string employeeName, string taskName, string description, DateTime dueDate, int priority);
        Task SendTaskDueSoonEmailAsync(string employeeEmail, string employeeName, string taskName, DateTime dueDate, int daysRemaining);
        Task SendTaskOverdueEmailAsync(string employeeEmail, string employeeName, string taskName, DateTime dueDate, int daysOverdue);
        Task SendDocumentApprovedEmailAsync(string employeeEmail, string employeeName, string documentName, string taskName);
        Task SendDocumentApprovedEmailAsync(string employeeEmail, string employeeName, string documentName, string taskName, string comments, string reviewerName);
        Task SendDocumentRejectedEmailAsync(string employeeEmail, string employeeName, string documentName, string taskName, string reason);
        Task SendDocumentRejectedEmailAsync(string employeeEmail, string employeeName, string documentName, string taskName, string reason, string reviewerName);
        Task SendOnboardingCompletedEmailAsync(string employeeEmail, string employeeName, DateTime completionDate);
    }
}
