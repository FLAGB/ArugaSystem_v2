using AndroidWebAPI.Data;
using AndroidWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AndroidWebAPI.Repositories
{
    public class QueueQRSettingRepository : IQueueQRSettingRepository
    {
        private readonly AppDbContext _context;

        public QueueQRSettingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<QueueQRSetting?> GetAsync()
        {
            return await _context.QueueQRSettings
                .FirstOrDefaultAsync();
        }

        public async Task<QueueQRSetting> UpdateAsync(bool isEnabled)
        {
            var setting = await _context.QueueQRSettings
                .FirstOrDefaultAsync();

            if (setting == null)
            {
                setting = new QueueQRSetting
                {
                    IsEnabled = isEnabled,
                    UpdatedAt = DateTime.Now
                };

                _context.QueueQRSettings.Add(setting);
            }
            else
            {
                setting.IsEnabled = isEnabled;
                setting.UpdatedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return setting;
        }
    }
}