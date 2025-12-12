using EmployeeOnboarding_DDMS.Domain.Entities;

namespace EmployeeOnboarding_DDMS.Domain.Interfaces
{
    public interface IDocumentRepository
    {
        // CRUD Operations
        Task<Document?> GetByIdAsync(int id);
        Task<IReadOnlyList<Document>> GetAllAsync();
        Task<Document> AddAsync(Document entity);
        Task UpdateAsync(Document entity);
        Task DeleteAsync(Document entity);
        
        // Custom Operations
        Task<IEnumerable<Document>> GetByTaskIdAsync(int taskId);
        Task<Document?> GetCurrentVersionAsync(int taskId);
        Task<IEnumerable<Document>> GetDocumentHistoryAsync(int taskId);
    }
}

