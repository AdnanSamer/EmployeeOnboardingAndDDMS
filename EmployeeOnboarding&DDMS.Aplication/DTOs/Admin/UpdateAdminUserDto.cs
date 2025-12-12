using System.ComponentModel.DataAnnotations;

namespace EmployeeOnboarding_DDMS.Aplication.DTOs.Admin
{
    public class UpdateAdminUserDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Range(0, 2)]
        public int Role { get; set; }

        public bool IsActive { get; set; } = true;
    }
}

