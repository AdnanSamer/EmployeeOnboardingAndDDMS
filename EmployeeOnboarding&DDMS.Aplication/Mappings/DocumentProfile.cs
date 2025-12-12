using AutoMapper;
using EmployeeOnboarding_DDMS.Aplication.DTOs.Documents;
using EmployeeOnboarding_DDMS.Domain.Entities;

namespace EmployeeOnboarding_DDMS.Aplication.Mappings
{
    public class DocumentProfile : Profile
    {
        public DocumentProfile()
        {
            CreateMap<Document, DocumentDto>();
        }
    }
}

