using AndroidWebAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AndroidWebAPI.DTOs;

namespace AndroidWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClinicOperatingScheduleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClinicOperatingScheduleController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ClinicOperatingSchedule
        [HttpGet]
        public async Task<IActionResult> GetSchedule()
        {
            var schedule = await _context.ClinicOperatingSchedules
                .Where(s => s.IsActive)
                .OrderBy(s => s.DayOfWeek)
                .ToListAsync();

            return Ok(schedule);
        }


        // PUT: api/ClinicOperatingSchedule/{scheduleID}
[HttpPut("{scheduleID}")]
public async Task<IActionResult> UpdateSchedule(
    int scheduleID,
    UpdateClinicOperatingScheduleDto dto)
{
    var schedule = await _context.ClinicOperatingSchedules
        .FirstOrDefaultAsync(s => s.ScheduleID == scheduleID);

    if (schedule == null)
    {
        return NotFound(new
        {
            message = "Clinic operating schedule not found."
        });
    }

    schedule.IsOpen = dto.IsOpen;
    schedule.OpeningTime = dto.OpeningTime;
    schedule.ClosingTime = dto.ClosingTime;
    schedule.QueueCutoffTime = dto.QueueCutoffTime;
    schedule.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();

    return Ok(schedule);
}
    }
}