using AutoMapper;
using EmployeeOnboarding_DDMS.Aplication.DTOs.Tasks;
using EmployeeOnboarding_DDMS.Aplication.Interfaces;
using EmployeeOnboarding_DDMS.Aplication.Wrappers;
using EmployeeOnboarding_DDMS.Domain.Entities;
using EmployeeOnboarding_DDMS.Domain.Interfaces;

namespace EmployeeOnboarding_DDMS.Aplication.Services
{
    public class TaskTemplateService : ITaskTemplateService
    {
        private readonly ITaskTemplateRepository _taskTemplateRepository;
        private readonly IMapper _mapper;

        public TaskTemplateService(ITaskTemplateRepository taskTemplateRepository, IMapper mapper)
        {
            _taskTemplateRepository = taskTemplateRepository;
            _mapper = mapper;
        }

        public async Task<Response<TaskTemplateDto>> CreateTemplateAsync(CreateTaskTemplateDto dto)
        {
            var template = _mapper.Map<TaskTemplate>(dto);
            var createdTemplate = await _taskTemplateRepository.AddAsync(template);
            var templateDto = _mapper.Map<TaskTemplateDto>(createdTemplate);

            return new Response<TaskTemplateDto>(templateDto, "Task template created successfully.");
        }

        public async Task<Response<TaskTemplateDto>> UpdateTemplateAsync(int id, CreateTaskTemplateDto dto)
        {
            var template = await _taskTemplateRepository.GetByIdAsync(id);
            if (template == null)
            {
                return new Response<TaskTemplateDto>("Task template not found.");
            }

            template.Name = dto.Name;
            template.Description = dto.Description;
            template.IsRequired = dto.IsRequired;
            template.RequiresDocumentUpload = dto.RequiresDocumentUpload;
            template.EstimatedDays = dto.EstimatedDays;

            await _taskTemplateRepository.UpdateAsync(template);
            var templateDto = _mapper.Map<TaskTemplateDto>(template);

            return new Response<TaskTemplateDto>(templateDto, "Task template updated successfully.");
        }

        public async Task<Response<IEnumerable<TaskTemplateDto>>> GetAllTemplatesAsync()
        {
            var templates = await _taskTemplateRepository.GetActiveTemplatesAsync();
            var templateDtos = _mapper.Map<IEnumerable<TaskTemplateDto>>(templates);

            return new Response<IEnumerable<TaskTemplateDto>>(templateDtos);
        }

        public async Task<Response<TaskTemplateDto>> GetTemplateByIdAsync(int id)
        {
            var template = await _taskTemplateRepository.GetByIdAsync(id);
            if (template == null)
            {
                return new Response<TaskTemplateDto>("Task template not found.");
            }

            var templateDto = _mapper.Map<TaskTemplateDto>(template);
            return new Response<TaskTemplateDto>(templateDto);
        }

        public async Task<Response<bool>> DeleteTemplateAsync(int id)
        {
            var template = await _taskTemplateRepository.GetByIdAsync(id);
            if (template == null)
            {
                return new Response<bool>("Task template not found.");
            }

            template.IsActive = false;
            await _taskTemplateRepository.UpdateAsync(template);

            return new Response<bool>(true, "Task template deleted successfully.");
        }
    }
}

