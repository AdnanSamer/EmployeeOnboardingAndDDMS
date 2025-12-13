using EmployeeOnboarding_DDMS.Domain.Entities;
using EmployeeOnboarding_DDMS.Domain.Interfaces;
using EmployeeOnboarding_DDMS.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace EmployeeOnboarding_DDMS.Infrastructure.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public DocumentRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Document?> GetByIdAsync(int id)
        {
            return await _dbContext.Documents.FindAsync(id);
        }

        public async Task<IReadOnlyList<Document>> GetAllAsync()
        {
            return await _dbContext.Documents.ToListAsync();
        }

        public async Task<Document> AddAsync(Document entity)
        {
            await _dbContext.Documents.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Document entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Document entity)
        {
            _dbContext.Documents.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Document>> GetByTaskIdAsync(int taskId)
        {
            return await _dbContext.Documents
                .Where(d => d.OnboardingTaskId == taskId)
                .OrderByDescending(d => d.Version)
                .ToListAsync();
        }

        public async Task<Document?> GetCurrentVersionAsync(int taskId)
        {
            return await _dbContext.Documents
                .Where(d => d.OnboardingTaskId == taskId && d.IsCurrentVersion)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Document>> GetDocumentHistoryAsync(int taskId)
        {
            return await _dbContext.Documents
                .Where(d => d.OnboardingTaskId == taskId)
                .OrderByDescending(d => d.Version)
                .ToListAsync();
        }
    }
}

