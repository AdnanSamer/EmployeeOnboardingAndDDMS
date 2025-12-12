using EmployeeOnboarding_DDMS.Domain.Entities;
using EmployeeOnboarding_DDMS.Domain.Enums;

namespace EmployeeOnboarding_DDMS.Domain.Interfaces
{
    public interface IEmployeeRepository
    {
        // CRUD Operations
        Task<Employee?> GetByIdAsync(int id);
        Task<IReadOnlyList<Employee>> GetAllAsync();
        Task<Employee> AddAsync(Employee entity);
        Task UpdateAsync(Employee entity);
        Task DeleteAsync(Employee entity);
        
        // Custom Operations
        Task<Employee?> GetByEmailAsync(string email);
        Task<Employee?> GetByUserIdAsync(int userId);
        Task<IEnumerable<Employee>> GetByDepartmentAsync(string department);
        Task<IEnumerable<Employee>> GetByOnboardingStatusAsync(OnboardingStatus status);
        Task<bool> EmailExistsAsync(string email);
    }
}

