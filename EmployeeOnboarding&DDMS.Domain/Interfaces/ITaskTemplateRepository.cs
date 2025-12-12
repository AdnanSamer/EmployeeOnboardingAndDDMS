using EmployeeOnboarding_DDMS.Domain.Entities;

namespace EmployeeOnboarding_DDMS.Domain.Interfaces
{
    public interface ITaskTemplateRepository
    {
        // CRUD Operations
        Task<TaskTemplate?> GetByIdAsync(int id);
        Task<IReadOnlyList<TaskTemplate>> GetAllAsync();
        Task<TaskTemplate> AddAsync(TaskTemplate entity);
        Task UpdateAsync(TaskTemplate entity);
        Task DeleteAsync(TaskTemplate entity);
        
        // Custom Operations
        Task<IEnumerable<TaskTemplate>> GetActiveTemplatesAsync();
    }
}

