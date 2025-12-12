using EmployeeOnboarding_DDMS.Aplication.DTOs.Admin;
using EmployeeOnboarding_DDMS.Aplication.Wrappers;

namespace EmployeeOnboarding_DDMS.Aplication.Interfaces
{
    public interface IAdminService
    {
        Task<PagedResponse<IEnumerable<AdminUserDto>>> GetUsersAsync(int pageNumber, int pageSize, string? search);
        Task<Response<AdminUserDto>> GetUserByIdAsync(int userId);
        Task<Response<AdminUserDto>> CreateUserAsync(CreateAdminUserDto dto);
        Task<Response<AdminUserDto>> UpdateUserAsync(int userId, UpdateAdminUserDto dto);
        Task<Response<bool>> DeleteUserAsync(int userId);
        Task<Response<bool>> ResetPasswordAsync(ResetPasswordAdminDto dto);

        Task<Response<IEnumerable<RoleDto>>> GetRolesAsync();
        Task<Response<IEnumerable<PermissionDto>>> GetPermissionsAsync();
        Task<Response<bool>> AssignPermissionsAsync(AssignPermissionsDto dto);

        Task<Response<SystemSettingsDto>> GetSettingsAsync();
        Task<Response<SystemSettingsDto>> UpdateSettingsAsync(SystemSettingsDto dto, string updatedBy);

        Task<PagedResponse<IEnumerable<ActivityLogDto>>> GetActivityLogsAsync(ActivityLogFilterDto filter);

        Task<Response<ChartDataDto>> GetOnboardingProgressChartAsync();
        Task<Response<ChartDataDto>> GetTaskCompletionChartAsync();
        Task<Response<ChartDataDto>> GetFileUploadsChartAsync();
        Task<Response<IEnumerable<OverdueTaskAlertDto>>> GetOverdueTasksAlertAsync();
        Task<Response<IEnumerable<RejectedDocumentAlertDto>>> GetRejectedDocumentsAlertAsync();
    }
}

