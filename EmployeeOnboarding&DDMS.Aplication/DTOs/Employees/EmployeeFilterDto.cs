using EmployeeOnboarding_DDMS.Domain.Enums;

namespace EmployeeOnboarding_DDMS.Aplication.DTOs.Employees
{
    public class EmployeeFilterDto
    {
        public string? Department { get; set; }
        public OnboardingStatus? OnboardingStatus { get; set; }
        public EmploymentStatus? EmploymentStatus { get; set; }
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

