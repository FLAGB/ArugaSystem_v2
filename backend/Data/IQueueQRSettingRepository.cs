using AndroidWebAPI.Models;

namespace AndroidWebAPI.Repositories
{
    public interface IQueueQRSettingRepository
    {
        Task<QueueQRSetting?> GetAsync();
        Task<QueueQRSetting> UpdateAsync(bool isEnabled);
    }
}