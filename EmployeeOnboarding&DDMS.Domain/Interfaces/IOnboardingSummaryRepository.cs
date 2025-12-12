using EmployeeOnboarding_DDMS.Domain.Entities;

namespace EmployeeOnboarding_DDMS.Domain.Interfaces
{
    public interface IOnboardingSummaryRepository
    {
        // CRUD Operations
        Task<OnboardingSummary?> GetByIdAsync(int id);
        Task<IReadOnlyList<OnboardingSummary>> GetAllAsync();
        Task<OnboardingSummary> AddAsync(OnboardingSummary entity);
        Task UpdateAsync(OnboardingSummary entity);
        Task DeleteAsync(OnboardingSummary entity);
        
        // Custom Operations
        Task<IEnumerable<OnboardingSummary>> GetByEmployeeIdAsync(int employeeId);
    }
}

