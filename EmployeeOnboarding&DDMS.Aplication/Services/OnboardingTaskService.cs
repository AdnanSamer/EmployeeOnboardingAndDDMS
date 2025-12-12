using AutoMapper;
using EmployeeOnboarding_DDMS.Aplication.DTOs.Tasks;
using EmployeeOnboarding_DDMS.Aplication.Interfaces;
using EmployeeOnboarding_DDMS.Aplication.Wrappers;
using EmployeeOnboarding_DDMS.Domain.Entities;
using EmployeeOnboarding_DDMS.Domain.Enums;
using EmployeeOnboarding_DDMS.Domain.Interfaces;



namespace EmployeeOnboarding_DDMS.Aplication.Services
{
    public class OnboardingTaskService : IOnboardingTaskService
    {
        private readonly IOnboardingTaskRepository _taskRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ITaskTemplateRepository _templateRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public OnboardingTaskService(
            IOnboardingTaskRepository taskRepository,
            IEmployeeRepository employeeRepository,
            ITaskTemplateRepository templateRepository,
            IDocumentRepository documentRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _taskRepository = taskRepository;
            _employeeRepository = employeeRepository;
            _templateRepository = templateRepository;
            _documentRepository = documentRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Response<TaskDto>> AssignTaskAsync(AssignTaskDto dto)
        {
            // CRITICAL FIX: Validate AssignedBy user exists
            var assigningUser = await _userRepository.GetByIdAsync(dto.AssignedBy);
            if (assigningUser == null)
            {
                return new Response<TaskDto>($"User with ID {dto.AssignedBy} not found. Cannot assign task.");
            }

            var employee = await _employeeRepository.GetByIdAsync(dto.EmployeeId);
            if (employee == null)
            {
                return new Response<TaskDto>("Employee not found.");
            }

            var template = await _templateRepository.GetByIdAsync(dto.TaskTemplateId);
            if (template == null)
            {
                return new Response<TaskDto>("Task template not found.");
            }

            var task = new OnboardingTask
            {
                EmployeeId = dto.EmployeeId,
                TaskTemplateId = dto.TaskTemplateId,
                AssignedBy = dto.AssignedBy,
                AssignedDate = DateTime.UtcNow,
                DueDate = dto.DueDate,
                Status = Domain.Enums.TaskStatus.Pending,
                Notes = dto.Notes
            };

            // Update employee onboarding status
            if (employee.OnboardingStatus == OnboardingStatus.NotStarted)
            {
                employee.OnboardingStatus = OnboardingStatus.InProgress;
                await _employeeRepository.UpdateAsync(employee);
            }

            var createdTask = await _taskRepository.AddAsync(task);
            var taskDto = await MapToTaskDtoAsync(createdTask);

            return new Response<TaskDto>(taskDto, "Task assigned successfully.");
        }

        public async Task<Response<TaskDto>> UpdateTaskStatusAsync(int id, UpdateTaskStatusDto dto)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
            {
                return new Response<TaskDto>("Task not found.");
            }

            // Business rule: Completed tasks cannot be modified unless reopened by HR
            if (task.Status == Domain.Enums.TaskStatus.Completed && dto.Status != Domain.Enums.TaskStatus.Completed)
            {
                return new Response<TaskDto>("Completed tasks cannot be modified. Please reopen the task first.");
            }

            // Business rule: Cannot mark as Completed if required documents are not approved
            if (dto.Status == Domain.Enums.TaskStatus.Completed)
            {
                var template = await _templateRepository.GetByIdAsync(task.TaskTemplateId);
                if (template?.RequiresDocumentUpload == true)
                {
                    var documents = await _documentRepository.GetByTaskIdAsync(id);
                    var hasApprovedDocument = documents.Any(d => d.Status == DocumentStatus.Approved);
                    
                    if (!hasApprovedDocument)
                    {
                        return new Response<TaskDto>("Task cannot be completed. At least one approved document is required.");
                    }
                }
                
                task.CompletedDate = DateTime.UtcNow;
            }
            else
            {
                task.CompletedDate = null;
            }

            task.Status = dto.Status;

            if (!string.IsNullOrEmpty(dto.Notes))
            {
                task.Notes = dto.Notes;
            }

            await _taskRepository.UpdateAsync(task);
            var taskDto = await MapToTaskDtoAsync(task);

            return new Response<TaskDto>(taskDto, "Task status updated successfully.");
        }

        public async Task<Response<IEnumerable<TaskDto>>> GetEmployeeTasksAsync(int employeeId)
        {
            var tasks = await _taskRepository.GetByEmployeeIdAsync(employeeId);
            var taskDtos = new List<TaskDto>();

            foreach (var task in tasks)
            {
                var taskDto = await MapToTaskDtoAsync(task);
                taskDtos.Add(taskDto);
            }

            return new Response<IEnumerable<TaskDto>>(taskDtos);
        }

        public async Task<Response<IEnumerable<EnhancedTaskDto>>> GetEnhancedEmployeeTasksAsync(int userId)
        {
            // CRITICAL FIX: userId is passed from frontend, need to find employee first
            var employee = await _employeeRepository.GetByUserIdAsync(userId);
            
            if (employee == null)
            {
                return new Response<IEnumerable<EnhancedTaskDto>>("Employee not found for this user.");
            }
            
            // Now get tasks using the actual EmployeeId
            var tasks = await _taskRepository.GetByEmployeeIdAsync(employee.Id);
            var enhancedTaskDtos = new List<EnhancedTaskDto>();

            foreach (var task in tasks)
            {
                var enhancedDto = MapToEnhancedTaskDto(task);
                enhancedTaskDtos.Add(enhancedDto);
            }

            return new Response<IEnumerable<EnhancedTaskDto>>(enhancedTaskDtos);
        }


        public async Task<Response<IEnumerable<TaskDto>>> GetAllTasksAsync()
        {
            var tasks = await _taskRepository.GetAllAsync();
            var taskDtos = new List<TaskDto>();

            foreach (var task in tasks.OrderByDescending(t => t.AssignedDate))
            {
                var taskDto = await MapToTaskDtoAsync(task);
                taskDtos.Add(taskDto);
            }

            return new Response<IEnumerable<TaskDto>>(taskDtos);
        }

        public async Task<Response<IEnumerable<TaskDto>>> GetOverdueTasksAsync()
        {
            var tasks = await _taskRepository.GetOverdueTasksAsync();
            var taskDtos = new List<TaskDto>();

            foreach (var task in tasks)
            {
                var taskDto = await MapToTaskDtoAsync(task);
                taskDtos.Add(taskDto);
            }

            return new Response<IEnumerable<TaskDto>>(taskDtos);
        }

        public async Task<Response<bool>> ReopenTaskAsync(int taskId, DTOs.Tasks.ReopenTaskDto dto)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
            {
                return new Response<bool>("Task not found.");
            }

            if (task.Status != Domain.Enums.TaskStatus.Completed)
            {
                return new Response<bool>("Only completed tasks can be reopened.");
            }

            task.Status = Domain.Enums.TaskStatus.Pending;
            task.CompletedDate = null;
            
            // Update due date if provided
            if (dto.NewDueDate.HasValue)
            {
                task.DueDate = dto.NewDueDate.Value;
            }

            // Store reopen reason in notes
            if (!string.IsNullOrWhiteSpace(dto.Reason))
            {
                task.Notes = string.IsNullOrWhiteSpace(task.Notes) 
                    ? $"Reopened: {dto.Reason}" 
                    : $"{task.Notes}\nReopened: {dto.Reason}";
            }

            await _taskRepository.UpdateAsync(task);

            return new Response<bool>(true, "Task reopened successfully.");
        }

        public async Task<Response<TaskDto>> GetTaskByIdAsync(int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
            {
                return new Response<TaskDto>("Task not found.");
            }

            var taskDto = await MapToTaskDtoAsync(task);
            return new Response<TaskDto>(taskDto);
        }

        private async Task<TaskDto> MapToTaskDtoAsync(OnboardingTask task)
        {
            var taskDto = _mapper.Map<TaskDto>(task);

            // Employee name
            if (task.Employee != null)
            {
                taskDto.EmployeeName = $"{task.Employee.FirstName} {task.Employee.LastName}";
            }
            else
            {
                var employee = await _employeeRepository.GetByIdAsync(task.EmployeeId);
                taskDto.EmployeeName = employee != null 
                    ? $"{employee.FirstName} {employee.LastName}" 
                    : "Unknown Employee";
            }

            // Task template details - CRITICAL FIX: Get from template only
            if (task.TaskTemplate != null)
            {
                taskDto.TaskTemplateName = task.TaskTemplate.Name;
                taskDto.Title = task.TaskTemplate.Name;
                taskDto.Description = task.TaskTemplate.Description ?? "";
                taskDto.TaskDescription = taskDto.Description;
                taskDto.RequiresDocumentUpload = task.TaskTemplate.RequiresDocumentUpload;
            }
            else
            {
                var template = await _templateRepository.GetByIdAsync(task.TaskTemplateId);
                if (template != null)
                {
                    taskDto.TaskTemplateName = template.Name;
                    taskDto.Title = template.Name;
                    taskDto.Description = template.Description ?? "";
                    taskDto.TaskDescription = taskDto.Description;
                    taskDto.RequiresDocumentUpload = template.RequiresDocumentUpload;
                }
                else
                {
                    taskDto.TaskTemplateName = "Unknown Template";
                    taskDto.Title = "Unknown Title";
                    taskDto.Description = "";
                    taskDto.TaskDescription = "";
                    taskDto.RequiresDocumentUpload = false;
                }
            }

            // Assigned by user details
            if (task.AssignedByUser != null)
            {
                taskDto.AssignedByName = $"{task.AssignedByUser.FirstName} {task.AssignedByUser.LastName}";
            }
            else if (task.AssignedBy > 0)
            {
                var assignedByUser = await _userRepository.GetByIdAsync(task.AssignedBy);
                taskDto.AssignedByName = assignedByUser != null
                    ? $"{assignedByUser.FirstName} {assignedByUser.LastName}"
                    : "Unknown";
            }
            else
            {
                taskDto.AssignedByName = "System";
            }

            taskDto.DocumentCount = task.Documents?.Count ?? 0;

            return taskDto;
        }

        private EnhancedTaskDto MapToEnhancedTaskDto(OnboardingTask task)
        {
            var now = DateTime.UtcNow;
            
            var enhancedDto = new EnhancedTaskDto
            {
                Id = task.Id,
                EmployeeId = task.EmployeeId,
                TaskTemplateId = task.TaskTemplateId,
                Status = task.Status,
                AssignedDate = task.AssignedDate,
                DueDate = task.DueDate,
                CompletedDate = task.CompletedDate,
                Priority = 1, // Default priority
                HrComments = task.Notes
            };

            // Employee name
            enhancedDto.EmployeeName = task.Employee != null 
                ? $"{task.Employee.FirstName} {task.Employee.LastName}" 
                : "Unknown Employee";

            // Task template details - CRITICAL FIX: Get from template only
            if (task.TaskTemplate != null)
            {
                enhancedDto.TaskTemplateName = task.TaskTemplate.Name;
                enhancedDto.Title = task.TaskTemplate.Name;
                enhancedDto.Description = task.TaskTemplate.Description ?? "";
                enhancedDto.TaskDescription = enhancedDto.Description;
                enhancedDto.RequiresDocumentUpload = task.TaskTemplate.RequiresDocumentUpload;
            }
            else
            {
                enhancedDto.TaskTemplateName = "Unknown Template";
                enhancedDto.Title = "Unknown Title";
                enhancedDto.Description = "";
                enhancedDto.TaskDescription = "";
                enhancedDto.RequiresDocumentUpload = false;
            }

            // Calculate overdue status
            enhancedDto.IsOverdue = task.DueDate < now && task.Status != Domain.Enums.TaskStatus.Completed;
            enhancedDto.DaysUntilDue = (int)(task.DueDate - now).TotalDays;
            enhancedDto.DaysOverdue = task.DueDate < now ? (int)(now - task.DueDate).TotalDays : 0;

            // Documents
            if (task.Documents != null && task.Documents.Any())
            {
                enhancedDto.Documents = task.Documents.Select(d => new TaskDocumentDto
                {
                    Id = d.Id,
                    FileName = d.OriginalFileName,
                    FileSize = d.FileSize,
                    Status = d.Status,
                    UploadDate = d.UploadDate,
                    ReviewedAt = d.ReviewedDate,
                    ReviewedBy = d.ReviewedByUser != null 
                        ? $"{d.ReviewedByUser.FirstName} {d.ReviewedByUser.LastName}" 
                        : null,
                    ReviewComments = d.ReviewComments,
                    Version = d.Version
                }).ToList();
            }

            // Permissions
            enhancedDto.CanUploadDocument = enhancedDto.RequiresDocumentUpload && 
                                           task.Status != Domain.Enums.TaskStatus.Completed;
            enhancedDto.CanMarkComplete = task.Status != Domain.Enums.TaskStatus.Completed;

            return enhancedDto;
        }
    }
}

