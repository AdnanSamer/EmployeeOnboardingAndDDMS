using EmployeeOnboarding_DDMS.Aplication.DTOs.Employee;
using EmployeeOnboarding_DDMS.Aplication.Interfaces;
using EmployeeOnboarding_DDMS.Aplication.Wrappers;
using EmployeeOnboarding_DDMS.Domain.Interfaces;

namespace EmployeeOnboarding_DDMS.Aplication.Services
{
    public class OnboardingSummaryService : IOnboardingSummaryService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IOnboardingTaskRepository _taskRepository;
        private readonly IDocumentRepository _documentRepository;

        public OnboardingSummaryService(
            IEmployeeRepository employeeRepository,
            IOnboardingTaskRepository taskRepository,
            IDocumentRepository documentRepository)
        {
            _employeeRepository = employeeRepository;
            _taskRepository = taskRepository;
            _documentRepository = documentRepository;
        }

        public async Task<Response<OnboardingSummaryDto>> GetOnboardingSummaryAsync(int employeeId)
        {
            Console.WriteLine($"[DEBUG] GetOnboardingSummaryAsync called with employeeId: {employeeId}");
            
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            
            Console.WriteLine($"[DEBUG] Employee lookup result: {(employee == null ? "NULL" : $"Found - Id={employee.Id}, Name={employee.FirstName} {employee.LastName}")}");
            
            if (employee == null)
            {
                Console.WriteLine($"[DEBUG] Returning 'Employee not found' response");
                return new Response<OnboardingSummaryDto>("Employee not found.");
            }

            var tasks = await _taskRepository.GetByEmployeeIdAsync(employeeId);
            var tasksList = tasks.ToList();

            // Calculate progress
            var totalTasks = tasksList.Count;
            var completedTasks = tasksList.Count(t => t.Status == Domain.Enums.TaskStatus.Completed);
            var progressPercentage = totalTasks > 0 ? (double)completedTasks / totalTasks * 100 : 0;

            var daysElapsed = (DateTime.UtcNow.Date - employee.HireDate).Days;
            var allCompleted = totalTasks > 0 && completedTasks == totalTasks;
            var completionDate = allCompleted ? tasksList.Max(t => t.CompletedDate) : null;

            // Build task summaries
            var taskSummaries = new List<TaskSummaryDto>();
            foreach (var task in tasksList.OrderBy(t => t.DueDate))
            {
                var documents = await _documentRepository.GetByTaskIdAsync(task.Id);
                var taskSummary = new TaskSummaryDto
                {
                    Title = task.TaskTemplate?.Name ?? "Unknown Task",
                    Status = task.Status.ToString(),
                    DueDate = task.DueDate,
                    CompletedDate = task.CompletedDate,
                    Documents = documents.Select(d => new DocumentSummaryItemDto
                    {
                        FileName = d.OriginalFileName,
                        Status = d.Status.ToString(),
                        ReviewComments = d.ReviewComments
                    }).ToList()
                };
                taskSummaries.Add(taskSummary);
            }

            // Build timeline
            var timeline = new List<TimelineEventDto>
            {
                new TimelineEventDto
                {
                    Date = employee.HireDate,
                    Event = "Onboarding Started",
                    Type = "milestone"
                }
            };

            // Add task completions to timeline
            foreach (var task in tasksList.Where(t => t.CompletedDate != null).OrderBy(t => t.CompletedDate))
            {
                timeline.Add(new TimelineEventDto
                {
                    Date = task.CompletedDate!.Value,
                    Event = $"Completed: {task.TaskTemplate?.Name ?? "Task"}",
                    Type = "task"
                });
            }

            if (allCompleted && completionDate.HasValue)
            {
                timeline.Add(new TimelineEventDto
                {
                    Date = completionDate.Value,
                    Event = "Onboarding Completed",
                    Type = "milestone"
                });
            }

            var summary = new OnboardingSummaryDto
            {
                Employee = new EmployeeInfoDto
                {
                    Id = employee.Id,
                    Name = $"{employee.FirstName} {employee.LastName}",
                    Email = employee.Email,
                    HireDate = employee.HireDate,
                    Department = employee.Department,
                    Position = employee.Position
                },
                OnboardingProgress = new OnboardingProgressSummaryDto
                {
                    StartDate = employee.HireDate,
                    CompletionDate = completionDate,
                    DaysElapsed = daysElapsed,
                    ProgressPercentage = Math.Round(progressPercentage, 2),
                    Status = allCompleted ? "Completed" : "InProgress"
                },
                Tasks = taskSummaries,
                Timeline = timeline.OrderBy(t => t.Date).ToList()
            };

            return new Response<OnboardingSummaryDto>(summary, "Onboarding summary retrieved successfully.");
        }
    }
}
