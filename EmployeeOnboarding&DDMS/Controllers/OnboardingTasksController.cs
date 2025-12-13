using EmployeeOnboarding_DDMS.Aplication.DTOs.Tasks;
using EmployeeOnboarding_DDMS.Aplication.Interfaces;
using EmployeeOnboarding_DDMS.Aplication.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeOnboarding_DDMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OnboardingTasksController : ControllerBase
    {
        private readonly IOnboardingTaskService _taskService;

        public OnboardingTasksController(IOnboardingTaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        [Authorize(Roles = "AdminHR")]
        public async Task<IActionResult> GetAllTasks()
        {
            var result = await _taskService.GetAllTasksAsync();
            
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Response<TaskDto>>> AssignTask([FromBody] AssignTaskDto dto)
        {
            var result = await _taskService.AssignTaskAsync(dto);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Response<TaskDto>>> GetTask(int id)
        {
            var result = await _taskService.GetTaskByIdAsync(id);
            if (!result.Succeeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<ActionResult<Response<IEnumerable<TaskDto>>>> GetEmployeeTasks(int employeeId)
        {
            var result = await _taskService.GetEmployeeTasksAsync(employeeId);
            return Ok(result);
        }

        [HttpGet("employee/{employeeId}/enhanced")]
        public async Task<ActionResult<Response<IEnumerable<EnhancedTaskDto>>>> GetEnhancedEmployeeTasks(int employeeId)
        {
            var result = await _taskService.GetEnhancedEmployeeTasksAsync(employeeId);
            return Ok(result);
        }

        [HttpGet("overdue")]
        public async Task<ActionResult<Response<IEnumerable<TaskDto>>>> GetOverdueTasks()
        {
            var result = await _taskService.GetOverdueTasksAsync();
            return Ok(result);
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult<Response<TaskDto>>> UpdateTaskStatus(int id, [FromBody] UpdateTaskStatusDto dto)
        {
            dto.TaskId = id;
            var result = await _taskService.UpdateTaskStatusAsync(id, dto);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("{id}/reopen")]
        public async Task<ActionResult<Response<bool>>> ReopenTask(int id, [FromBody] ReopenTaskDto dto)
        {
            var result = await _taskService.ReopenTaskAsync(id, dto);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}

