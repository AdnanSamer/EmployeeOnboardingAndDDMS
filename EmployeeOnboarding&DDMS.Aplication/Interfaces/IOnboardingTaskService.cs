using EmployeeOnboarding_DDMS.Aplication.DTOs.Tasks;
using EmployeeOnboarding_DDMS.Aplication.Wrappers;

namespace EmployeeOnboarding_DDMS.Aplication.Interfaces
{
    public interface IOnboardingTaskService
    {
        Task<Response<TaskDto>> AssignTaskAsync(AssignTaskDto dto);
        Task<Response<TaskDto>> UpdateTaskStatusAsync(int id, UpdateTaskStatusDto dto);
        Task<Response<IEnumerable<TaskDto>>> GetEmployeeTasksAsync(int employeeId);
        Task<Response<IEnumerable<EnhancedTaskDto>>> GetEnhancedEmployeeTasksAsync(int userId);
        Task<Response<IEnumerable<TaskDto>>> GetAllTasksAsync();
        Task<Response<IEnumerable<TaskDto>>> GetOverdueTasksAsync();
        Task<Response<bool>> ReopenTaskAsync(int taskId, ReopenTaskDto dto);
        Task<Response<TaskDto>> GetTaskByIdAsync(int id);
    }
}

