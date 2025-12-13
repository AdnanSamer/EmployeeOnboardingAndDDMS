using AutoMapper;
using EmployeeOnboarding_DDMS.Aplication.DTOs.Dashboard;
using EmployeeOnboarding_DDMS.Aplication.Interfaces;
using EmployeeOnboarding_DDMS.Aplication.Wrappers;
using EmployeeOnboarding_DDMS.Domain.Enums;
using EmployeeOnboarding_DDMS.Domain.Interfaces;
using EmployeeOnboarding_DDMS.Domain.Entities;

namespace EmployeeOnboarding_DDMS.Aplication.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IOnboardingTaskRepository _taskRepository;
        private readonly IDocumentRepository _documentRepository;

        public DashboardService(
            IEmployeeRepository employeeRepository,
            IOnboardingTaskRepository taskRepository,
            IDocumentRepository documentRepository)
        {
            _employeeRepository = employeeRepository;
            _taskRepository = taskRepository;
            _documentRepository = documentRepository;
        }

        public async Task<Response<DashboardStatsDto>> GetDashboardStatsAsync()
        {
            try
            {
                var employees = await _employeeRepository.GetAllAsync();
                var tasks = await _taskRepository.GetAllAsync();
                var documents = await _documentRepository.GetAllAsync();

                var employeeList = employees?.ToList() ?? new List<Domain.Entities.Employee>();
                var taskList = tasks?.ToList() ?? new List<Domain.Entities.OnboardingTask>();
                var documentList = documents?.ToList() ?? new List<Domain.Entities.Document>();

                var overdueTasks = await _taskRepository.GetOverdueTasksAsync();
                var overdueTaskList = overdueTasks?.ToList() ?? new List<Domain.Entities.OnboardingTask>();

                var stats = new DashboardStatsDto
                {
                    TotalEmployees = employeeList.Count,
                    EmployeesInOnboarding = employeeList.Count(e => e.OnboardingStatus == OnboardingStatus.InProgress),
                    CompletedOnboardings = employeeList.Count(e => e.OnboardingStatus == OnboardingStatus.Completed),
                    PendingTasks = taskList.Count(t => t.Status == Domain.Enums.TaskStatus.Pending),
                    OverdueTasks = overdueTaskList.Count,
                    CompletedTasks = taskList.Count(t => t.Status == Domain.Enums.TaskStatus.Completed),
                    TotalDocuments = documentList.Count,
                    PendingDocumentReviews = documentList.Count(d => d.Status == DocumentStatus.Pending)
                };

                return new Response<DashboardStatsDto>(stats);
            }
            catch (Exception ex)
            {
                return new Response<DashboardStatsDto>($"Error retrieving dashboard statistics: {ex.Message}");
            }
        }

        public async Task<Response<IEnumerable<EmployeeProgressDto>>> GetEmployeeProgressAsync()
        {
            var employees = await _employeeRepository.GetByOnboardingStatusAsync(OnboardingStatus.InProgress);
            var progressList = new List<EmployeeProgressDto>();

            foreach (var employee in employees)
            {
                var tasks = await _taskRepository.GetByEmployeeIdAsync(employee.Id);
                var taskList = tasks.ToList();

                var completedCount = taskList.Count(t => t.Status == Domain.Enums.TaskStatus.Completed);
                var totalCount = taskList.Count;
                
                var completionPercentage = totalCount > 0
                    ? (int)Math.Round((double)completedCount / totalCount * 100)
                    : 0;

                var progress = new EmployeeProgressDto
                {
                    EmployeeId = employee.Id,
                    EmployeeName = $"{employee.FirstName} {employee.LastName}",
                    Email = employee.Email,
                    Department = employee.Department,
                    TotalTasks = totalCount,
                    CompletedTasks = completedCount,
                    PendingTasks = taskList.Count(t => t.Status == Domain.Enums.TaskStatus.Pending),
                    OverdueTasks = taskList.Count(t => t.DueDate < DateTime.UtcNow && t.Status != Domain.Enums.TaskStatus.Completed),
                    ProgressPercentage = completionPercentage,
                    CompletionPercentage = completionPercentage,
                    OnboardingCompletedDate = employee.OnboardingCompletedDate
                };

                progressList.Add(progress);
            }

            return new Response<IEnumerable<EmployeeProgressDto>>(progressList);
        }
    }
}

