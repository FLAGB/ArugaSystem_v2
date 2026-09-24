using AndroidWebAPI.Models;

namespace AndroidWebAPI.Repositories
{
    public interface IQueueRepository
    {
        Task<List<Queue>> GetAllAsync();
        Task<Queue?> GetByIdAsync(Guid queueId);
        Task<Queue?> GetParentQueueAsync(Guid parentId, DateTime queueDate);

        Task<Queue> CreateAsync(Queue queue);

        Task UpdateAsync(Queue queue);
        Task DeleteAsync(Guid queueId);

        Task<int> GetNextQueueNumberAsync(DateTime queueDate);
    }
}