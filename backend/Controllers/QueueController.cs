using AndroidWebAPI.Models;
using AndroidWebAPI.Repositories;
using AndroidWebAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AndroidWebAPI.DTOs;

namespace AndroidWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QueueController : ControllerBase
    {
        private readonly IQueueRepository _queueRepository;
        private readonly AppDbContext _context;

        public QueueController(IQueueRepository queueRepository, AppDbContext context)
        {
            _queueRepository = queueRepository;
            _context = context;
        }

        // ============================================================
        // DOCTOR DASHBOARD BOARD — added for the Doctor/Nurse module.
        // Operates on the same Queues/QueueChildren tables as the rest of
        // this controller, going through _context directly (EF) instead
        // of IQueueRepository since these need shapes/updates the existing
        // repository methods don't provide (a flattened per-child board,
        // and "call next"/"complete" state transitions).
        //
        // A Queues row is one parent visit and can have several
        // QueueChildren. The board below shows one row per child, but
        // call-next/complete act on the whole visit (Queues.Status) —
        // completing marks every child in that visit as done together.
        // ============================================================

        private static DateTime TodayStart => DateTime.Today;
        private static DateTime TodayEnd => DateTime.Today.AddDays(1);

        // GET /api/Queue/board
        // Polled by the Doctor dashboard to show who's waiting / in progress.
        [HttpGet("board")]
        public async Task<IActionResult> GetBoard()
        {
            var queues = await _context.Queues
                .Include(q => q.Parent)
                .Include(q => q.QueueChildren).ThenInclude(qc => qc.Child)
                .Where(q => q.QueueDate >= TodayStart && q.QueueDate < TodayEnd
                            && (q.Status == "Waiting" || q.Status == "InProgress"))
                .OrderBy(q => q.QueueNumber)
                .ToListAsync();

            var board = queues
                .SelectMany(q => q.QueueChildren.DefaultIfEmpty(), (q, qc) => new
                {
                    QueueID = q.QueueID,
                    QueueNumber = q.QueueNumber,
                    QueueStatus = q.Status,
                    AssignedRoomID = q.AssignedRoomID,
                    ChildId = qc != null ? (Guid?)qc.ChildID : null,
                    ChildName = qc?.Child != null ? $"{qc.Child.FirstName} {qc.Child.LastName}" : null,
                    ParentName = q.Parent != null ? $"{q.Parent.FirstName} {q.Parent.LastName}" : null,
                    // Not tracked per-child in this schema yet — the Doctor
                    // modal falls back to a manual pick when these are null.
                    RecordId = (Guid?)null,
                    VaccineName = (string?)null,
                    DoseNumber = (int?)null,
                });

            return Ok(board);
        }

        // PATCH /api/Queue/call-next
        // Doctor button: bring the next Waiting visit into a room.
        [HttpPatch("call-next")]
        public async Task<IActionResult> CallNext([FromQuery] int? roomId)
        {
            var current = await _context.Queues
                .FirstOrDefaultAsync(q => q.QueueDate >= TodayStart && q.QueueDate < TodayEnd && q.Status == "InProgress");
            if (current != null)
            {
                // Only one "now serving" visit at a time — finish it first.
                return Ok(new { message = "A visit is already in progress.", queueID = current.QueueID });
            }

            var next = await _context.Queues
                .Where(q => q.QueueDate >= TodayStart && q.QueueDate < TodayEnd && q.Status == "Waiting")
                .OrderBy(q => q.QueueNumber)
                .FirstOrDefaultAsync();

            if (next == null) return Ok(new { message = "No one waiting." });

            next.Status = "InProgress";
            next.AssignedRoomID = roomId;
            next.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return Ok(next);
        }

        // PATCH /api/Queue/{queueId}/complete
        // Doctor button: mark the current visit done (auto-calls the next one).
        [HttpPatch("{queueId}/complete")]
        public async Task<IActionResult> Complete(Guid queueId)
        {
            var entry = await _context.Queues.FindAsync(queueId);
            if (entry == null) return NotFound();

            entry.Status = "Completed";
            entry.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            // Auto-advance: call the next Waiting visit forward.
            var stillInProgress = await _context.Queues
                .AnyAsync(q => q.QueueDate >= TodayStart && q.QueueDate < TodayEnd && q.Status == "InProgress");
            if (!stillInProgress)
            {
                var next = await _context.Queues
                    .Where(q => q.QueueDate >= TodayStart && q.QueueDate < TodayEnd && q.Status == "Waiting")
                    .OrderBy(q => q.QueueNumber)
                    .FirstOrDefaultAsync();
                if (next != null)
                {
                    next.Status = "InProgress";
                    next.AssignedRoomID = entry.AssignedRoomID;
                    next.UpdatedAt = DateTime.Now;
                    await _context.SaveChangesAsync();
                }
            }

            return Ok(entry);
        }

        // GET: api/Queue/today
        // Same response shape as GetAll (grouped-by-visit, with children),
        // but filtered to today server-side instead of pulling every queue
        // entry the clinic has ever had. Use this for "today's queue" views
        // (e.g. Staff Dashboard); use GetAll where full history is wanted
        // (e.g. Queue Management, Reports).
        [HttpGet("today")]
        public async Task<IActionResult> GetToday()
        {
            var queues = await _context.Queues
                .Include(q => q.Parent)
                .Include(q => q.QueueChildren).ThenInclude(qc => qc.Child)
                .Where(q => q.QueueDate >= TodayStart && q.QueueDate < TodayEnd)
                .OrderBy(q => q.QueueNumber)
                .ToListAsync();

            var result = queues.Select(q => new QueueResponseDto
            {
                QueueID = q.QueueID,
                QueueNumber = q.QueueNumber,
                BarangayNo = q.Parent?.BarangayNo,
                RequestBy = $"{q.Parent?.FirstName} {q.Parent?.LastName}".Trim(),
                Status = q.Status,
                QueueDate = q.QueueDate,

                Children = q.QueueChildren
                    .Select(qc => new QueueChildResponseDto
                    {
                        ChildID = qc.ChildID,
                        Name = $"{qc.Child?.FirstName} {qc.Child?.LastName}".Trim()
                    })
                    .ToList()
            }).ToList();

            return Ok(result);
        }

       // GET: api/Queue
[HttpGet]
public async Task<IActionResult> GetAll()
{
    var queues = await _queueRepository.GetAllAsync();

    var result = queues.Select(q => new QueueResponseDto
    {
        QueueID = q.QueueID,
        QueueNumber = q.QueueNumber,
        BarangayNo = q.Parent?.BarangayNo,
        RequestBy = $"{q.Parent?.FirstName} {q.Parent?.LastName}".Trim(),
        Status = q.Status,
        QueueDate = q.QueueDate,

        Children = q.QueueChildren
            .Select(qc => new QueueChildResponseDto
            {
                ChildID = qc.ChildID,
                Name = $"{qc.Child?.FirstName} {qc.Child?.LastName}".Trim()
            })
            .ToList()
    }).ToList();

    return Ok(result);
}

        // GET: api/Queue/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var queue = await _queueRepository.GetByIdAsync(id);

            if (queue == null)
                return NotFound();

            var result = new QueueResponseDto
{
    QueueID = queue.QueueID,
    QueueNumber = queue.QueueNumber,
    BarangayNo = queue.Parent?.BarangayNo,
    RequestBy = $"{queue.Parent?.FirstName} {queue.Parent?.LastName}".Trim(),
    Status = queue.Status,
    QueueDate = queue.QueueDate,

    Children = queue.QueueChildren
        .Select(qc => new QueueChildResponseDto
        {
            ChildID = qc.ChildID,
            Name = $"{qc.Child?.FirstName} {qc.Child?.LastName}".Trim()
        })
        .ToList()
};

return Ok(result);
        }

        // GET: api/Queue/parent/{parentId}
        [HttpGet("parent/{parentId}")]
        public async Task<IActionResult> GetParentQueue(Guid parentId)
        {
            var queue = await _queueRepository.GetParentQueueAsync(
                parentId,
                DateTime.Today
            );

            if (queue == null)
                return NotFound(new
                {
                    message = "Parent does not have a queue entry for today."
                });

            var result = new QueueResponseDto
{
    QueueID = queue.QueueID,
    QueueNumber = queue.QueueNumber,
    BarangayNo = queue.Parent?.BarangayNo,
    RequestBy = $"{queue.Parent?.FirstName} {queue.Parent?.LastName}".Trim(),
    Status = queue.Status,
    QueueDate = queue.QueueDate,

    Children = queue.QueueChildren
        .Select(qc => new QueueChildResponseDto
        {
            ChildID = qc.ChildID,
            Name = $"{qc.Child?.FirstName} {qc.Child?.LastName}".Trim()
        })
        .ToList()
};

return Ok(result);
        }

        // POST: api/Queue
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateQueueRequest request)
        {
            if (request.ChildIDs == null || request.ChildIDs.Count == 0)
            {
                return BadRequest(new
                {
                    message = "At least one child must be selected."
                });
            }

            var today = DateTime.Today;

            // Prevent duplicate queue entry for the same parent today
            var existingQueue =
                await _queueRepository.GetParentQueueAsync(
                    request.ParentID,
                    today
                );

            if (existingQueue != null)
            {
                return Conflict(new
                {
                    message = "This parent already has a queue entry for today.",
                    queueID = existingQueue.QueueID,
                    queueNumber = existingQueue.QueueNumber
                });
            }

            var nextQueueNumber =
                await _queueRepository.GetNextQueueNumberAsync(today);

            var queue = new Queue
            {
                QueueID = Guid.NewGuid(),
                ParentID = request.ParentID,
                QueueNumber = nextQueueNumber,
                QueueDate = today,
                Status = "Waiting",
                CreatedAt = DateTime.Now
            };

            foreach (var childId in request.ChildIDs.Distinct())
            {
                queue.QueueChildren.Add(new QueueChild
                {
                    QueueChildID = Guid.NewGuid(),
                    QueueID = queue.QueueID,
                    ChildID = childId,
                    CreatedAt = DateTime.Now
                });
            }

            var createdQueue =
                await _queueRepository.CreateAsync(queue);

            var result =
                await _queueRepository.GetByIdAsync(createdQueue.QueueID);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdQueue.QueueID },
                result
            );
        }

        // PUT: api/Queue/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(
            Guid id,
            [FromBody] UpdateQueueStatusRequest request)
        {
            var queue = await _queueRepository.GetByIdAsync(id);

            if (queue == null)
                return NotFound();

            queue.Status = request.Status;
            queue.UpdatedAt = DateTime.Now;

            await _queueRepository.UpdateAsync(queue);

            // Map to the same DTO every other endpoint here returns —
            // returning the raw entity serializes its Parent/QueueChildren
            // navigation properties, which can hit a circular reference
            // (Parent -> Queues -> ... ) and throw *after* the update has
            // already been saved, surfacing as a 500 even though the write
            // succeeded.
            var result = new QueueResponseDto
            {
                QueueID = queue.QueueID,
                QueueNumber = queue.QueueNumber,
                BarangayNo = queue.Parent?.BarangayNo,
                RequestBy = $"{queue.Parent?.FirstName} {queue.Parent?.LastName}".Trim(),
                Status = queue.Status,
                QueueDate = queue.QueueDate,

                Children = queue.QueueChildren
                    .Select(qc => new QueueChildResponseDto
                    {
                        ChildID = qc.ChildID,
                        Name = $"{qc.Child?.FirstName} {qc.Child?.LastName}".Trim()
                    })
                    .ToList()
            };

            return Ok(result);
        }

        // DELETE: api/Queue/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var queue = await _queueRepository.GetByIdAsync(id);

            if (queue == null)
                return NotFound();

            await _queueRepository.DeleteAsync(id);

            return NoContent();
        }
    }

    // ============================================
    // REQUEST DTOs
    // ============================================

    public class CreateQueueRequest
    {
        public Guid ParentID { get; set; }

        public List<Guid> ChildIDs { get; set; }
            = new List<Guid>();
    }

    public class UpdateQueueStatusRequest
    {
        public string Status { get; set; } = "Waiting";
    }
}