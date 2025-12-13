using EmployeeOnboarding_DDMS.Aplication.DTOs.Employees;
using EmployeeOnboarding_DDMS.Aplication.Interfaces;
using EmployeeOnboarding_DDMS.Aplication.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeOnboarding_DDMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpPost]
        public async Task<ActionResult<Response<EmployeeDto>>> CreateEmployee([FromBody] CreateEmployeeDto dto)
        {
            var result = await _employeeService.CreateEmployeeAsync(dto);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Response<EmployeeDto>>> GetEmployee(int id)
        {
            var result = await _employeeService.GetEmployeeByIdAsync(id);
            if (!result.Succeeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResponse<IEnumerable<EmployeeDto>>>> GetEmployees([FromQuery] EmployeeFilterDto filter)
        {
            var result = await _employeeService.GetEmployeesAsync(filter);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Response<EmployeeDto>>> UpdateEmployee(int id, [FromBody] UpdateEmployeeDto dto)
        {
            dto.Id = id;
            var result = await _employeeService.UpdateEmployeeAsync(id, dto);
            if (!result.Succeeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Response<bool>>> DeactivateEmployee(int id)
        {
            var result = await _employeeService.DeactivateEmployeeAsync(id);
            if (!result.Succeeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPost("{id}/complete-onboarding")]
        public async Task<ActionResult<Response<bool>>> CompleteOnboarding(int id)
        {
            var result = await _employeeService.CompleteOnboardingAsync(id);
            if (!result.Succeeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
    }
}

