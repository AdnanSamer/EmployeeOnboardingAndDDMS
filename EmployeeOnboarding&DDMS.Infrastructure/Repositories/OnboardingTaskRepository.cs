using EmployeeOnboarding_DDMS.Domain.Entities;
using EmployeeOnboarding_DDMS.Domain.Enums;
using EmployeeOnboarding_DDMS.Domain.Interfaces;
using EmployeeOnboarding_DDMS.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace EmployeeOnboarding_DDMS.Infrastructure.Repositories
{
    public class OnboardingTaskRepository : IOnboardingTaskRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public OnboardingTaskRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // CRUD Operations
        public async Task<OnboardingTask?> GetByIdAsync(int id)
        {
            return await _dbContext.OnboardingTasks
                .Include(t => t.Employee)
                .Include(t => t.TaskTemplate)
                .Include(t => t.Documents)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IReadOnlyList<OnboardingTask>> GetAllAsync()
        {
            return await _dbContext.OnboardingTasks.ToListAsync();
        }

        public async Task<OnboardingTask> AddAsync(OnboardingTask entity)
        {
            await _dbContext.OnboardingTasks.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(OnboardingTask entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(OnboardingTask entity)
        {
            _dbContext.OnboardingTasks.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        // Custom Operations
        public async Task<IEnumerable<OnboardingTask>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _dbContext.OnboardingTasks
                .Include(t => t.Employee)
                .Include(t => t.TaskTemplate)
                .Include(t => t.Documents)
                .Where(t => t.EmployeeId == employeeId)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<OnboardingTask>> GetOverdueTasksAsync()
        {
            var today = DateTime.UtcNow.Date;
            return await _dbContext.OnboardingTasks
                .Include(t => t.Employee)
                .Include(t => t.TaskTemplate)
                .Where(t => t.DueDate < today && 
                           t.Status != Domain.Enums.TaskStatus.Completed && 
                           t.Status != Domain.Enums.TaskStatus.Canceled)
                .ToListAsync();
        }

        public async Task<IEnumerable<OnboardingTask>> GetTasksByStatusAsync(EmployeeOnboarding_DDMS.Domain.Enums.TaskStatus status)
        {
            return await _dbContext.OnboardingTasks
                .Include(t => t.Employee)
                .Include(t => t.TaskTemplate)
                .Where(t => t.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<OnboardingTask>> GetTasksByEmployeeAndStatusAsync(int employeeId, EmployeeOnboarding_DDMS.Domain.Enums.TaskStatus status)
        {
            return await _dbContext.OnboardingTasks
                .Include(t => t.TaskTemplate)
                .Where(t => t.EmployeeId == employeeId && t.Status == status)
                .ToListAsync();
        }
    }
}

