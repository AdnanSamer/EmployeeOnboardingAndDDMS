using EmployeeOnboarding_DDMS.Domain.Common;

namespace EmployeeOnboarding_DDMS.Domain.Entities
{
    public class Permission : AuditableBaseEntity
    {
        public string PermissionName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}

