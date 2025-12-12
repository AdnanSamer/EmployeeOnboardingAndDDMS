using System.ComponentModel.DataAnnotations;

namespace EmployeeOnboarding_DDMS.Aplication.DTOs.Admin
{
    public class AssignPermissionsDto
    {
        [Range(0, 2)]
        public int RoleId { get; set; }

        [Required]
        public List<int> PermissionIds { get; set; } = new();
    }
}

