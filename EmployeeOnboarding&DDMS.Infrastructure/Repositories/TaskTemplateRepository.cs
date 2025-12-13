using EmployeeOnboarding_DDMS.Domain.Entities;
using EmployeeOnboarding_DDMS.Domain.Interfaces;
using EmployeeOnboarding_DDMS.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace EmployeeOnboarding_DDMS.Infrastructure.Repositories
{
    public class TaskTemplateRepository : ITaskTemplateRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public TaskTemplateRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<TaskTemplate?> GetByIdAsync(int id)
        {
            return await _dbContext.TaskTemplates.FindAsync(id);
        }

        public async Task<IReadOnlyList<TaskTemplate>> GetAllAsync()
        {
            return await _dbContext.TaskTemplates.ToListAsync();
        }

        public async Task<TaskTemplate> AddAsync(TaskTemplate entity)
        {
            await _dbContext.TaskTemplates.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(TaskTemplate entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(TaskTemplate entity)
        {
            _dbContext.TaskTemplates.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<IEnumerable<TaskTemplate>> GetActiveTemplatesAsync()
        {
            return await _dbContext.TaskTemplates
                .Where(t => t.IsActive)
                .ToListAsync();
        }
    }
}

