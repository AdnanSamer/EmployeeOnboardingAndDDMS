using EmployeeOnboarding_DDMS.Domain.Entities;
using EmployeeOnboarding_DDMS.Domain.Interfaces;
using EmployeeOnboarding_DDMS.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace EmployeeOnboarding_DDMS.Infrastructure.Repositories
{
    public class SystemSettingRepository : ISystemSettingRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public SystemSettingRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SystemSetting?> GetByKeyAsync(string key)
        {
            return await _dbContext.SystemSettings.FirstOrDefaultAsync(s => s.SettingKey == key);
        }

        public async Task<SystemSetting> UpsertAsync(string key, string settingValue, string updatedBy = "System")
        {
            var existing = await _dbContext.SystemSettings.FirstOrDefaultAsync(s => s.SettingKey == key);

            if (existing == null)
            {
                var entity = new SystemSetting
                {
                    SettingKey = key,
                    SettingValue = settingValue,
                    CreatedBy = updatedBy,
                    LastModifiedBy = updatedBy
                };
                await _dbContext.SystemSettings.AddAsync(entity);
                await _dbContext.SaveChangesAsync();
                return entity;
            }

            existing.SettingValue = settingValue;
            existing.LastModifiedBy = updatedBy;
            _dbContext.SystemSettings.Update(existing);
            await _dbContext.SaveChangesAsync();
            return existing;
        }
    }
}

