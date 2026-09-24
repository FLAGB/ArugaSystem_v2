using AndroidWebAPI.Models;

namespace AndroidWebAPI.Services
{
    public interface IQueueQRCodeService
    {
        Task<QueueQRCode?> GenerateTodayAsync();
        Task<QueueQRCode?> GetActiveQRCodeAsync();
    }
}