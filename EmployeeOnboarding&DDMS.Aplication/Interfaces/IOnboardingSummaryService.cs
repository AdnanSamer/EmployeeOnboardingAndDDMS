using EmployeeOnboarding_DDMS.Aplication.DTOs.Employee;
using EmployeeOnboarding_DDMS.Aplication.Wrappers;

namespace EmployeeOnboarding_DDMS.Aplication.Interfaces
{
    public interface IOnboardingSummaryService
    {
        Task<Response<OnboardingSummaryDto>> GetOnboardingSummaryAsync(int employeeId);
    }
}
