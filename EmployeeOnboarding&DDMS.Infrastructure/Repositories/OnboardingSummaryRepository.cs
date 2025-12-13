using EmployeeOnboarding_DDMS.Domain.Entities;
using EmployeeOnboarding_DDMS.Domain.Interfaces;
using EmployeeOnboarding_DDMS.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace EmployeeOnboarding_DDMS.Infrastructure.Repositories
{
    public class OnboardingSummaryRepository : IOnboardingSummaryRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public OnboardingSummaryRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<OnboardingSummary?> GetByIdAsync(int id)
        {
            return await _dbContext.OnboardingSummaries.FindAsync(id);
        }

        public async Task<IReadOnlyList<OnboardingSummary>> GetAllAsync()
        {
            return await _dbContext.OnboardingSummaries.ToListAsync();
        }

        public async Task<OnboardingSummary> AddAsync(OnboardingSummary entity)
        {
            await _dbContext.OnboardingSummaries.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(OnboardingSummary entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(OnboardingSummary entity)
        {
            _dbContext.OnboardingSummaries.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<IEnumerable<OnboardingSummary>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _dbContext.OnboardingSummaries
                .Where(s => s.EmployeeId == employeeId)
                .OrderByDescending(s => s.GeneratedDate)
                .ToListAsync();
        }
    }
}

