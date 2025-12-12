using EmployeeOnboarding_DDMS.Domain.Entities;
using EmployeeOnboarding_DDMS.Domain.Enums;

namespace EmployeeOnboarding_DDMS.Domain.Interfaces
{
    public interface IOnboardingTaskRepository
    {
        // CRUD Operations
        Task<OnboardingTask?> GetByIdAsync(int id);
        Task<IReadOnlyList<OnboardingTask>> GetAllAsync();
        Task<OnboardingTask> AddAsync(OnboardingTask entity);
        Task UpdateAsync(OnboardingTask entity);
        Task DeleteAsync(OnboardingTask entity);
        
        // Custom Operations
        Task<IEnumerable<OnboardingTask>> GetByEmployeeIdAsync(int employeeId);
        Task<IEnumerable<OnboardingTask>> GetOverdueTasksAsync();
        Task<IEnumerable<OnboardingTask>> GetTasksByStatusAsync(Enums.TaskStatus status);
        Task<IEnumerable<OnboardingTask>> GetTasksByEmployeeAndStatusAsync(int employeeId, Enums.TaskStatus status);
    }
}

