using EmployeeOnboarding_DDMS.Aplication.DTOs.Tasks;
using EmployeeOnboarding_DDMS.Aplication.Interfaces;
using EmployeeOnboarding_DDMS.Aplication.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeOnboarding_DDMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskTemplatesController : ControllerBase
    {
        private readonly ITaskTemplateService _taskTemplateService;

        public TaskTemplatesController(ITaskTemplateService taskTemplateService)
        {
            _taskTemplateService = taskTemplateService;
        }

        [HttpGet]
        public async Task<ActionResult<Response<IEnumerable<TaskTemplateDto>>>> GetAllTemplates()
        {
            var result = await _taskTemplateService.GetAllTemplatesAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Response<TaskTemplateDto>>> GetTemplate(int id)
        {
            var result = await _taskTemplateService.GetTemplateByIdAsync(id);
            if (!result.Succeeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "AdminHR")]
        public async Task<ActionResult<Response<TaskTemplateDto>>> CreateTemplate([FromBody] CreateTaskTemplateDto dto)
        {
            var result = await _taskTemplateService.CreateTemplateAsync(dto);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "AdminHR")]
        public async Task<ActionResult<Response<TaskTemplateDto>>> UpdateTemplate(int id, [FromBody] CreateTaskTemplateDto dto)
        {
            var result = await _taskTemplateService.UpdateTemplateAsync(id, dto);
            if (!result.Succeeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "AdminHR")]
        public async Task<ActionResult<Response<bool>>> DeleteTemplate(int id)
        {
            var result = await _taskTemplateService.DeleteTemplateAsync(id);
            if (!result.Succeeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
    }
}

