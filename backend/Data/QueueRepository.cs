using AndroidWebAPI.Data;
using AndroidWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AndroidWebAPI.Repositories
{
    public class QueueRepository : IQueueRepository
    {
        private readonly AppDbContext _context;

        public QueueRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Queue>> GetAllAsync()
        {
            return await _context.Queues
                .Include(q => q.Parent)
                .Include(q => q.QueueChildren)
                    .ThenInclude(qc => qc.Child)
                .OrderBy(q => q.QueueDate)
                .ThenBy(q => q.QueueNumber)
                .ToListAsync();
        }

        public async Task<Queue?> GetByIdAsync(Guid queueId)
        {
            return await _context.Queues
                .Include(q => q.Parent)
                .Include(q => q.QueueChildren)
                    .ThenInclude(qc => qc.Child)
                .FirstOrDefaultAsync(q => q.QueueID == queueId);
        }

        public async Task<Queue?> GetParentQueueAsync(
            Guid parentId,
            DateTime queueDate)
        {
            return await _context.Queues
                .Include(q => q.Parent)
                .Include(q => q.QueueChildren)
                    .ThenInclude(qc => qc.Child)
                .FirstOrDefaultAsync(q =>
                    q.ParentID == parentId &&
                    q.QueueDate.Date == queueDate.Date);
        }

        public async Task<int> GetNextQueueNumberAsync(DateTime queueDate)
        {
            var lastQueueNumber = await _context.Queues
                .Where(q => q.QueueDate.Date == queueDate.Date)
                .Select(q => (int?)q.QueueNumber)
                .MaxAsync();

            return (lastQueueNumber ?? 0) + 1;
        }

        public async Task<Queue> CreateAsync(Queue queue)
        {
            _context.Queues.Add(queue);
            await _context.SaveChangesAsync();

            return queue;
        }

        public async Task UpdateAsync(Queue queue)
        {
            _context.Queues.Update(queue);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid queueId)
        {
            var queue = await _context.Queues
                .FirstOrDefaultAsync(q => q.QueueID == queueId);

            if (queue == null)
                return;

            _context.Queues.Remove(queue);
            await _context.SaveChangesAsync();
        }
    }
}