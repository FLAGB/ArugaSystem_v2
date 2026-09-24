using AndroidWebAPI.Data;
using AndroidWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AndroidWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VaccinationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VaccinationController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/Vaccination/GetRooms
        // Shape expected by StaffDashboard.vue: { roomId, roomName, doctorName, isOccupied }
        [HttpGet("GetRooms")]
        public async Task<IActionResult> GetRooms()
        {
            var rooms = await _context.ClinicRooms.ToListAsync();

            var doctorIds = rooms
                .Where(r => r.AssignedDoctorID.HasValue)
                .Select(r => r.AssignedDoctorID!.Value)
                .Distinct()
                .ToList();

            var doctors = await _context.Users
                .Where(u => doctorIds.Contains(u.UserID))
                .ToDictionaryAsync(u => u.UserID, u => $"{u.FirstName} {u.LastName}".Trim());

            var result = rooms.Select(r => new
            {
                roomId = r.RoomID,
                roomName = r.RoomNumber,
                doctorName = r.AssignedDoctorID.HasValue && doctors.ContainsKey(r.AssignedDoctorID.Value)
                    ? doctors[r.AssignedDoctorID.Value]
                    : null,
                isOccupied = r.IsOccupied
            });

            return Ok(result);
        }

        // POST /api/Vaccination/AssignRoom
        // Body: { queueId, roomId }
        // Occupies the room AND sets Queue.AssignedRoomID/Status so the
        // Doctor board (GET /api/Queue/board) agrees with the Staff view.
        [HttpPost("AssignRoom")]
        public async Task<IActionResult> AssignRoom([FromBody] AssignRoomRequest request)
        {
            var room = await _context.ClinicRooms.FindAsync(request.RoomId);
            if (room == null)
                return NotFound(new { message = "Room not found." });

            if (room.IsOccupied)
                return Conflict(new { message = "That room is already occupied." });

            var queueEntry = await _context.Queues
                .Include(q => q.QueueChildren)
                .FirstOrDefaultAsync(q => q.QueueID == request.QueueId);

            if (queueEntry == null)
                return NotFound(new { message = "Queue entry not found." });

            // Free up any room this visit was previously assigned to, so a
            // reassignment doesn't leave a stale room stuck as occupied.
            if (queueEntry.AssignedRoomID.HasValue && queueEntry.AssignedRoomID != room.RoomID)
            {
                var oldRoom = await _context.ClinicRooms.FindAsync(queueEntry.AssignedRoomID.Value);
                if (oldRoom != null)
                {
                    oldRoom.IsOccupied = false;
                    oldRoom.CurrentChildID = null;
                }
            }

            room.IsOccupied = true;
            room.CurrentChildID = queueEntry.QueueChildren.FirstOrDefault()?.ChildID;

            queueEntry.Status = "InProgress";
            queueEntry.AssignedRoomID = room.RoomID;
            queueEntry.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Assigned.", roomId = room.RoomID, queueId = queueEntry.QueueID });
        }

        // POST /api/Vaccination/CompleteSession/{roomId}
        // Frees the room AND marks the matching Queue entry Completed.
        [HttpPost("CompleteSession/{roomId}")]
        public async Task<IActionResult> CompleteSession(int roomId)
        {
            var room = await _context.ClinicRooms.FindAsync(roomId);
            if (room == null)
                return NotFound(new { message = "Room not found." });

            var queueEntry = await _context.Queues
                .FirstOrDefaultAsync(q => q.AssignedRoomID == roomId && q.Status == "InProgress");

            room.IsOccupied = false;
            room.CurrentChildID = null;

            if (queueEntry != null)
            {
                queueEntry.Status = "Completed";
                queueEntry.UpdatedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Session completed.", roomId });
        }
    }

    public class AssignRoomRequest
    {
        public Guid QueueId { get; set; }
        public int RoomId { get; set; }
    }
}