using System.ComponentModel.DataAnnotations;

namespace EmployeeOnboarding_DDMS.Aplication.DTOs.Admin
{
    public class ResetPasswordAdminDto
    {
        [Required]
        public int UserId { get; set; }

        [Required, MinLength(8)]
        public string NewPassword { get; set; } = string.Empty;
    }
}

