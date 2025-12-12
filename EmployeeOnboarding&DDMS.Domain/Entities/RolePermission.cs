using EmployeeOnboarding_DDMS.Domain.Common;
using EmployeeOnboarding_DDMS.Domain.Enums;

namespace EmployeeOnboarding_DDMS.Domain.Entities
{
    public class RolePermission : AuditableBaseEntity
    {
        public int Role { get; set; }
        public int PermissionId { get; set; }

        public virtual Permission Permission { get; set; } = null!;
    }
}

