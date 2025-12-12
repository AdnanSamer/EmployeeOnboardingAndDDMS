using EmployeeOnboarding_DDMS.Aplication.DTOs.Dashboard;
using EmployeeOnboarding_DDMS.Aplication.Interfaces;
using EmployeeOnboarding_DDMS.Aplication.Wrappers;
using EmployeeOnboarding_DDMS.Domain.Interfaces;
using TaskStatus = EmployeeOnboarding_DDMS.Domain.Enums.TaskStatus;
using DocumentStatus = EmployeeOnboarding_DDMS.Domain.Enums.DocumentStatus;


namespace EmployeeOnboarding_DDMS.Aplication.Services
{
    public class EmployeeDashboardService : IEmployeeDashboardService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IOnboardingTaskRepository _taskRepository;
        private readonly IDocumentRepository _documentRepository;

        public EmployeeDashboardService(
            IEmployeeRepository employeeRepository,
            IOnboardingTaskRepository taskRepository,
            IDocumentRepository documentRepository)
        {
            _employeeRepository = employeeRepository;
            _taskRepository = taskRepository;
            _documentRepository = documentRepository;
        }

        public async Task<Response<EmployeeDashboardDto>> GetDashboardDataAsync(int userId)
        {
            // Get employee by user ID
            var employee = await _employeeRepository.GetByUserIdAsync(userId);
            if (employee == null)
            {
                return new Response<EmployeeDashboardDto>("Employee not found for this user.");
            }

            // Get all tasks for employee
            var tasks = await _taskRepository.GetByEmployeeIdAsync(employee.Id);
            var tasksList = tasks.ToList();

            // Calculate onboarding progress
            var totalTasks = tasksList.Count;
            var completedTasks = tasksList.Count(t => t.Status == TaskStatus.Completed);
            var pendingTasks = tasksList.Count(t => t.Status == TaskStatus.Pending);
            var inProgressTasks = tasksList.Count(t => t.Status == TaskStatus.InProgress);
            var overdueTasks = tasksList.Count(t => 
                t.DueDate < DateTime.UtcNow.Date && 
                t.Status != TaskStatus.Completed && 
                t.Status != TaskStatus.Canceled);

            var progressPercentage = totalTasks > 0 ? (double)completedTasks / totalTasks * 100 : 0;

            // Estimate completion date (simple calculation based on average completion rate)
            DateTime? estimatedCompletion = null;
            if (completedTasks > 0 && completedTasks < totalTasks)
            {
                var daysElapsed = (DateTime.UtcNow.Date - employee.HireDate).Days;
                if (daysElapsed > 0)
                {
                    var tasksPerDay = (double)completedTasks / daysElapsed;
                    var remainingTasks = totalTasks - completedTasks;
                    var estimatedDaysRemaining = (int)Math.Ceiling(remainingTasks / tasksPerDay);
                    estimatedCompletion = DateTime.UtcNow.Date.AddDays(estimatedDaysRemaining);
                }
            }

            // Get all documents for employee's tasks
            var allDocuments = new List<Domain.Entities.Document>();
            foreach (var task in tasksList)
            {
                var docs = await _documentRepository.GetByTaskIdAsync(task.Id);
                allDocuments.AddRange(docs);
            }

            var totalDocuments = allDocuments.Count;
            var approvedDocuments = allDocuments.Count(d => d.Status == DocumentStatus.Approved);
            var pendingDocuments = allDocuments.Count(d => d.Status == DocumentStatus.Pending);
            var rejectedDocuments = allDocuments.Count(d => d.Status == DocumentStatus.Rejected);

            // Get recent activity (last 5 events)
            var recentActivity = new List<ActivityDto>();
            
            // Add completed tasks
            foreach (var task in tasksList.Where(t => t.CompletedDate != null).OrderByDescending(t => t.CompletedDate).Take(3))
            {
                recentActivity.Add(new ActivityDto
                {
                    Type = "TaskCompleted",
                    Message = $"Completed task: {task.TaskTemplate?.Name ?? "Unknown Task"}",
                    Date = task.CompletedDate!.Value,
                    Severity = "success"
                });
            }

            // Add recent document reviews
            foreach (var doc in allDocuments.Where(d => d.ReviewedDate != null).OrderByDescending(d => d.ReviewedDate).Take(2))
            {
                var severity = doc.Status == DocumentStatus.Approved ? "success" : "danger";
                var action = doc.Status == DocumentStatus.Approved ? "approved" : "rejected";
                recentActivity.Add(new ActivityDto
                {
                    Type = "DocumentReviewed",
                    Message = $"Your document '{doc.OriginalFileName}' was {action}",
                    Date = doc.ReviewedDate!.Value,
                    Severity = severity
                });
            }

            // Sort and take top 5
            recentActivity = recentActivity.OrderByDescending(a => a.Date).Take(5).ToList();

            // Get upcoming deadlines (next 5 tasks by due date)
            var upcomingDeadlines = tasksList
                .Where(t => t.Status != TaskStatus.Completed && t.Status != TaskStatus.Canceled)
                .OrderBy(t => t.DueDate)
                .Take(5)
                .Select(t => new UpcomingDeadlineDto
                {
                    TaskId = t.Id,
                    TaskTitle = t.TaskTemplate?.Name ?? "Unknown Task",
                    DueDate = t.DueDate,
                    DaysRemaining = (t.DueDate - DateTime.UtcNow.Date).Days
                })
                .ToList();

            var dashboard = new EmployeeDashboardDto
            {
                OnboardingProgress = new OnboardingProgressDto
                {
                    TotalTasks = totalTasks,
                    CompletedTasks = completedTasks,
                    PendingTasks = pendingTasks,
                    InProgressTasks = inProgressTasks,
                    OverdueTasks = overdueTasks,
                    ProgressPercentage = Math.Round(progressPercentage, 2),
                    EstimatedCompletionDate = estimatedCompletion
                },
                DocumentSummary = new DocumentSummaryDto
                {
                    TotalDocuments = totalDocuments,
                    ApprovedDocuments = approvedDocuments,
                    PendingDocuments = pendingDocuments,
                    RejectedDocuments = rejectedDocuments
                },
                RecentActivity = recentActivity,
                UpcomingDeadlines = upcomingDeadlines
            };

            return new Response<EmployeeDashboardDto>(dashboard, "Dashboard data retrieved successfully.");
        }
    }
}
