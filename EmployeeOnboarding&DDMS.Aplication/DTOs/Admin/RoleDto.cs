namespace EmployeeOnboarding_DDMS.Aplication.DTOs.Admin
{
    public class RoleDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public IEnumerable<PermissionDto> Permissions { get; set; } = Enumerable.Empty<PermissionDto>();
    }
}

