using EmployeeOnboarding_DDMS.Aplication.DTOs.Employees;
using EmployeeOnboarding_DDMS.Aplication.Wrappers;

namespace EmployeeOnboarding_DDMS.Aplication.Interfaces
{
    public interface IEmployeeService
    {
        Task<Response<EmployeeDto>> CreateEmployeeAsync(CreateEmployeeDto dto);
        Task<Response<EmployeeDto>> UpdateEmployeeAsync(int id, UpdateEmployeeDto dto);
        Task<Response<EmployeeDto>> GetEmployeeByIdAsync(int id);
        Task<PagedResponse<IEnumerable<EmployeeDto>>> GetEmployeesAsync(EmployeeFilterDto filter);
        Task<Response<bool>> DeactivateEmployeeAsync(int id);
        Task<Response<bool>> CompleteOnboardingAsync(int employeeId);
    }
}

