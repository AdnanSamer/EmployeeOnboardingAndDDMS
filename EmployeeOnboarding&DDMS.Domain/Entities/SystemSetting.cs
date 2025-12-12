using EmployeeOnboarding_DDMS.Domain.Common;

namespace EmployeeOnboarding_DDMS.Domain.Entities
{
    public class SystemSetting : AuditableBaseEntity
    {
        public string SettingKey { get; set; } = string.Empty;
        public string SettingValue { get; set; } = string.Empty;
    }
}

