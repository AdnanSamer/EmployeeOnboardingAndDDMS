using EmployeeOnboarding_DDMS.Aplication.DTOs.Dashboard;
using EmployeeOnboarding_DDMS.Aplication.Wrappers;

namespace EmployeeOnboarding_DDMS.Aplication.Interfaces
{
    public interface IEmployeeDashboardService
    {
        Task<Response<EmployeeDashboardDto>> GetDashboardDataAsync(int userId);
    }
}
