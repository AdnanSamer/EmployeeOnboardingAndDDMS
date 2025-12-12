using EmployeeOnboarding_DDMS.Domain.Common;
using EmployeeOnboarding_DDMS.Domain.Enums;

namespace EmployeeOnboarding_DDMS.Domain.Entities
{
    public class Employee : AuditableBaseEntity
    {
        public int? UserId { get; set; }
        public string EmployeeNumber { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime HireDate { get; set; }
        public string? Department { get; set; }
        public string? Position { get; set; }
        public EmploymentStatus EmploymentStatus { get; set; }
        public OnboardingStatus OnboardingStatus { get; set; }
        public DateTime? OnboardingCompletedDate { get; set; }
        
        // Address fields
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }

        // Navigation properties
        public virtual User? User { get; set; }
        public virtual ICollection<OnboardingTask> OnboardingTasks { get; set; } = new List<OnboardingTask>();
        public virtual ICollection<OnboardingSummary> OnboardingSummaries { get; set; } = new List<OnboardingSummary>();
    }
}

