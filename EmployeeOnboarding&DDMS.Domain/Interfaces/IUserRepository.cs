using EmployeeOnboarding_DDMS.Domain.Entities;

namespace EmployeeOnboarding_DDMS.Domain.Interfaces
{
    public interface IUserRepository
    {
        // CRUD Operations
        Task<User?> GetByIdAsync(int id);
        Task<IReadOnlyList<User>> GetAllAsync();
        Task<User> AddAsync(User entity);
        Task UpdateAsync(User entity);
        Task DeleteAsync(User entity);
        
        // Custom Operations
        Task<User?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<(IReadOnlyList<User> Users, int TotalRecords)> GetPagedAsync(int pageNumber, int pageSize, string? search);
    }
}

