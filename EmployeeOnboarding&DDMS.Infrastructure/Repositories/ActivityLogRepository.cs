using EmployeeOnboarding_DDMS.Domain.Entities;
using EmployeeOnboarding_DDMS.Domain.Interfaces;
using EmployeeOnboarding_DDMS.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace EmployeeOnboarding_DDMS.Infrastructure.Repositories
{
    public class ActivityLogRepository : IActivityLogRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ActivityLogRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ActivityLog> AddAsync(ActivityLog log)
        {
            await _dbContext.ActivityLogs.AddAsync(log);
            await _dbContext.SaveChangesAsync();
            return log;
        }

        public async Task<(IReadOnlyList<ActivityLog> Logs, int TotalRecords)> GetPagedAsync(
            int? userId,
            string? action,
            string? entityType,
            DateTime? startDate,
            DateTime? endDate,
            int pageNumber,
            int pageSize)
        {
            var query = _dbContext.ActivityLogs.AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(l => l.UserId == userId.Value);
            }

            if (!string.IsNullOrWhiteSpace(action))
            {
                var lowered = action.ToLower();
                query = query.Where(l => l.Action.ToLower().Contains(lowered));
            }

            if (!string.IsNullOrWhiteSpace(entityType))
            {
                var lowered = entityType.ToLower();
                query = query.Where(l => l.EntityType.ToLower().Contains(lowered));
            }

            if (startDate.HasValue)
            {
                query = query.Where(l => l.Timestamp >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(l => l.Timestamp <= endDate.Value);
            }

            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderByDescending(l => l.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalRecords);
        }
    }
}

