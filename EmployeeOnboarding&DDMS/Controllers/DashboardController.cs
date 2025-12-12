using EmployeeOnboarding_DDMS.Aplication.DTOs.Dashboard;
using EmployeeOnboarding_DDMS.Aplication.Interfaces;
using EmployeeOnboarding_DDMS.Aplication.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeOnboarding_DDMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Get dashboard statistics
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult<Response<DashboardStatsDto>>> GetDashboardStats()
        {
            try
            {
                var result = await _dashboardService.GetDashboardStatsAsync();
                if (!result.Succeeded)
                {
                    return StatusCode(500, result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<DashboardStatsDto>($"An error occurred: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get employee progress for all employees in onboarding
        /// </summary>
        [HttpGet("progress")]
        public async Task<ActionResult<Response<IEnumerable<EmployeeProgressDto>>>> GetEmployeeProgress()
        {
            var result = await _dashboardService.GetEmployeeProgressAsync();
            return Ok(result);
        }
    }
}

