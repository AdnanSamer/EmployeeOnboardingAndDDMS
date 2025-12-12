using EmployeeOnboarding_DDMS.Aplication.DTOs.Auth;
using EmployeeOnboarding_DDMS.Aplication.Wrappers;

namespace EmployeeOnboarding_DDMS.Aplication.Interfaces
{
    public interface IAuthService
    {
        Task<Response<AuthResponseDto>> LoginAsync(LoginDto loginDto);
        Task<Response<AuthResponseDto>> RegisterAsync(RegisterDto registerDto);
        Task<Response<bool>> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
    }
}

