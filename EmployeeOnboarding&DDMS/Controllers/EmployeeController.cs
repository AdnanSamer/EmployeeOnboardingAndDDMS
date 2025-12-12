using EmployeeOnboarding_DDMS.Aplication.DTOs.Dashboard;
using EmployeeOnboarding_DDMS.Aplication.DTOs.Employee;
using EmployeeOnboarding_DDMS.Aplication.Interfaces;
using EmployeeOnboarding_DDMS.Aplication.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeOnboarding_DDMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeDashboardService _dashboardService;
        private readonly IOnboardingSummaryService _summaryService;

        public EmployeeController(
            IEmployeeDashboardService dashboardService,
            IOnboardingSummaryService summaryService)
        {
            _dashboardService = dashboardService;
            _summaryService = summaryService;
        }

        /// <summary>
        /// Get employee dashboard data
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<ActionResult<Response<EmployeeDashboardDto>>> GetDashboard()
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            if (userId == 0)
            {
                return Unauthorized(new Response<EmployeeDashboardDto>("Invalid token."));
            }

            var result = await _dashboardService.GetDashboardDataAsync(userId);
            if (!result.Succeeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get employee onboarding summary
        /// </summary>
        [HttpGet("{id}/onboarding-summary")]
        public async Task<ActionResult<Response<OnboardingSummaryDto>>> GetOnboardingSummary(int id)
        {
            var result = await _summaryService.GetOnboardingSummaryAsync(id);
            if (!result.Succeeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}
