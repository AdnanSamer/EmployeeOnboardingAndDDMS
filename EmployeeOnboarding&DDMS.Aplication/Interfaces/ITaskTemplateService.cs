using EmployeeOnboarding_DDMS.Aplication.DTOs.Tasks;
using EmployeeOnboarding_DDMS.Aplication.Wrappers;

namespace EmployeeOnboarding_DDMS.Aplication.Interfaces
{
    public interface ITaskTemplateService
    {
        Task<Response<TaskTemplateDto>> CreateTemplateAsync(CreateTaskTemplateDto dto);
        Task<Response<TaskTemplateDto>> UpdateTemplateAsync(int id, CreateTaskTemplateDto dto);
        Task<Response<IEnumerable<TaskTemplateDto>>> GetAllTemplatesAsync();
        Task<Response<TaskTemplateDto>> GetTemplateByIdAsync(int id);
        Task<Response<bool>> DeleteTemplateAsync(int id);
    }
}

