using EmployeeOnboarding_DDMS.Aplication.DTOs.Admin;
using EmployeeOnboarding_DDMS.Aplication.Interfaces;
using EmployeeOnboarding_DDMS.Aplication.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeeOnboarding_DDMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("users")]
        public async Task<ActionResult<PagedResponse<IEnumerable<AdminUserDto>>>> GetUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
        {
            var result = await _adminService.GetUsersAsync(pageNumber, pageSize, search);
            return Ok(result);
        }

        [HttpGet("users/{userId}")]
        public async Task<ActionResult<Response<AdminUserDto>>> GetUser(int userId)
        {
            var result = await _adminService.GetUserByIdAsync(userId);
            if (!result.Succeeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPost("users")]
        public async Task<ActionResult<Response<AdminUserDto>>> CreateUser([FromBody] CreateAdminUserDto dto)
        {
            var result = await _adminService.CreateUserAsync(dto);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("users/{userId}")]
        public async Task<ActionResult<Response<AdminUserDto>>> UpdateUser(int userId, [FromBody] UpdateAdminUserDto dto)
        {
            var result = await _adminService.UpdateUserAsync(userId, dto);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("users/{userId}")]
        public async Task<ActionResult<Response<bool>>> DeleteUser(int userId)
        {
            var result = await _adminService.DeleteUserAsync(userId);
            if (!result.Succeeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPost("users/reset-password")]
        public async Task<ActionResult<Response<bool>>> ResetPassword([FromBody] ResetPasswordAdminDto dto)
        {
            var result = await _adminService.ResetPasswordAsync(dto);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("roles")]
        public async Task<ActionResult<Response<IEnumerable<RoleDto>>>> GetRoles()
        {
            var result = await _adminService.GetRolesAsync();
            return Ok(result);
        }

        [HttpGet("permissions")]
        public async Task<ActionResult<Response<IEnumerable<PermissionDto>>>> GetPermissions()
        {
            var result = await _adminService.GetPermissionsAsync();
            return Ok(result);
        }

        [HttpPost("roles/assign-permissions")]
        public async Task<ActionResult<Response<bool>>> AssignPermissions([FromBody] AssignPermissionsDto dto)
        {
            var result = await _adminService.AssignPermissionsAsync(dto);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("settings")]
        public async Task<ActionResult<Response<SystemSettingsDto>>> GetSettings()
        {
            var result = await _adminService.GetSettingsAsync();
            return Ok(result);
        }

        [HttpPut("settings")]
        public async Task<ActionResult<Response<SystemSettingsDto>>> UpdateSettings([FromBody] SystemSettingsDto dto)
        {
            var updatedBy = User?.FindFirstValue(ClaimTypes.Name) ?? "Admin";
            var result = await _adminService.UpdateSettingsAsync(dto, updatedBy);
            return Ok(result);
        }

        [HttpGet("activity-logs")]
        public async Task<ActionResult<PagedResponse<IEnumerable<ActivityLogDto>>>> GetActivityLogs([FromQuery] ActivityLogFilterDto filter)
        {
            var result = await _adminService.GetActivityLogsAsync(filter);
            return Ok(result);
        }

        [HttpGet("dashboard/charts/onboarding-progress")]
        public async Task<ActionResult<Response<ChartDataDto>>> GetOnboardingProgressChart()
        {
            var result = await _adminService.GetOnboardingProgressChartAsync();
            return Ok(result);
        }

        [HttpGet("dashboard/charts/task-completion")]
        public async Task<ActionResult<Response<ChartDataDto>>> GetTaskCompletionChart()
        {
            var result = await _adminService.GetTaskCompletionChartAsync();
            return Ok(result);
        }

        [HttpGet("dashboard/charts/file-uploads")]
        public async Task<ActionResult<Response<ChartDataDto>>> GetFileUploadsChart()
        {
            var result = await _adminService.GetFileUploadsChartAsync();
            return Ok(result);
        }

        [HttpGet("dashboard/alerts/overdue-tasks")]
        public async Task<ActionResult<Response<IEnumerable<OverdueTaskAlertDto>>>> GetOverdueTaskAlerts()
        {
            var result = await _adminService.GetOverdueTasksAlertAsync();
            return Ok(result);
        }

        [HttpGet("dashboard/alerts/rejected-documents")]
        public async Task<ActionResult<Response<IEnumerable<RejectedDocumentAlertDto>>>> GetRejectedDocumentAlerts()
        {
            var result = await _adminService.GetRejectedDocumentsAlertAsync();
            return Ok(result);
        }
    }
}

