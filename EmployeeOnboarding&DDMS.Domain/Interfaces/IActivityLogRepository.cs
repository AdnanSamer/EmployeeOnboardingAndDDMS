using EmployeeOnboarding_DDMS.Domain.Entities;

namespace EmployeeOnboarding_DDMS.Domain.Interfaces
{
    public interface IActivityLogRepository
    {
        Task<ActivityLog> AddAsync(ActivityLog log);
        Task<(IReadOnlyList<ActivityLog> Logs, int TotalRecords)> GetPagedAsync(
            int? userId,
            string? action,
            string? entityType,
            DateTime? startDate,
            DateTime? endDate,
            int pageNumber,
            int pageSize);
    }
}

