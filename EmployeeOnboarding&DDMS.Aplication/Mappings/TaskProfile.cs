using AutoMapper;
using EmployeeOnboarding_DDMS.Aplication.DTOs.Tasks;
using EmployeeOnboarding_DDMS.Domain.Entities;

namespace EmployeeOnboarding_DDMS.Aplication.Mappings
{
    public class TaskProfile : Profile
    {
        public TaskProfile()
        {
            CreateMap<TaskTemplate, TaskTemplateDto>().ReverseMap();
            CreateMap<CreateTaskTemplateDto, TaskTemplate>();
            CreateMap<OnboardingTask, TaskDto>()
                .ForMember(dest => dest.EmployeeName, opt => opt.Ignore())
                .ForMember(dest => dest.TaskTemplateName, opt => opt.Ignore())
                .ForMember(dest => dest.Title, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.Ignore())
                .ForMember(dest => dest.TaskDescription, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedByName, opt => opt.Ignore())
                .ForMember(dest => dest.Priority, opt => opt.Ignore())
                .ForMember(dest => dest.RequiresDocumentUpload, opt => opt.Ignore())
                .ForMember(dest => dest.DocumentCount, opt => opt.Ignore());
            CreateMap<AssignTaskDto, OnboardingTask>();
        }
    }
}

