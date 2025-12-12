using EmployeeOnboarding_DDMS.Domain.Entities;

namespace EmployeeOnboarding_DDMS.Domain.Interfaces
{
    public interface ISystemSettingRepository
    {
        Task<SystemSetting?> GetByKeyAsync(string key);
        Task<SystemSetting> UpsertAsync(string key, string settingValue, string updatedBy = "System");
    }
}

