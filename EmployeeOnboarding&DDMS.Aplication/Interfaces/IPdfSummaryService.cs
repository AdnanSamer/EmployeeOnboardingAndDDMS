using EmployeeOnboarding_DDMS.Aplication.Wrappers;

namespace EmployeeOnboarding_DDMS.Aplication.Interfaces
{
    public interface IPdfSummaryService
    {
        Task<Response<byte[]>> GenerateOnboardingSummaryAsync(int employeeId);
    }
}

