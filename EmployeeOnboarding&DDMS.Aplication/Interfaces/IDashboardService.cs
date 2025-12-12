using EmployeeOnboarding_DDMS.Aplication.DTOs.Dashboard;
using EmployeeOnboarding_DDMS.Aplication.Wrappers;

namespace EmployeeOnboarding_DDMS.Aplication.Interfaces
{
    public interface IDashboardService
    {
        Task<Response<DashboardStatsDto>> GetDashboardStatsAsync();
        Task<Response<IEnumerable<EmployeeProgressDto>>> GetEmployeeProgressAsync();
    }
}

