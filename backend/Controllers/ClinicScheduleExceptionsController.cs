using AndroidWebAPI.Data;
using AndroidWebAPI.DTOs;
using AndroidWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AndroidWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClinicScheduleExceptionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClinicScheduleExceptionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ClinicScheduleExceptions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClinicScheduleException>>> GetExceptions()
        {
            var exceptions = await _context.ClinicScheduleExceptions
                .Where(e => e.IsActive)
                .OrderBy(e => e.ExceptionDate)
                .ToListAsync();

            return Ok(exceptions);
        }

        // GET: api/ClinicScheduleExceptions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClinicScheduleException>> GetException(int id)
        {
            var exception = await _context.ClinicScheduleExceptions
                .FirstOrDefaultAsync(e => e.ExceptionID == id);

            if (exception == null)
                return NotFound(new { message = "Schedule exception not found." });

            return Ok(exception);
        }

        // GET: api/ClinicScheduleExceptions/date/2026-08-26
        [HttpGet("date/{date}")]
        public async Task<ActionResult<ClinicScheduleException>> GetExceptionByDate(DateTime date)
        {
            var exception = await _context.ClinicScheduleExceptions
                .FirstOrDefaultAsync(e =>
                    e.ExceptionDate.Date == date.Date &&
                    e.IsActive);

            if (exception == null)
                return NotFound(new { message = "No active exception exists for this date." });

            return Ok(exception);
        }

        // POST: api/ClinicScheduleExceptions
        [HttpPost]
        public async Task<ActionResult<ClinicScheduleException>> CreateException(
            ClinicScheduleExceptionDto dto)
        {
            var existing = await _context.ClinicScheduleExceptions
                .FirstOrDefaultAsync(e =>
                    e.ExceptionDate.Date == dto.ExceptionDate.Date &&
                    e.IsActive);

            if (existing != null)
            {
                return Conflict(new
                {
                    message = "An active schedule exception already exists for this date."
                });
            }

            var exception = new ClinicScheduleException
            {
                ExceptionDate = dto.ExceptionDate.Date,
                IsOpen = dto.IsOpen,
                OpeningTime = dto.OpeningTime,
                ClosingTime = dto.ClosingTime,
                QueueCutoffTime = dto.QueueCutoffTime,
                Reason = dto.Reason,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.ClinicScheduleExceptions.Add(exception);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetException),
                new { id = exception.ExceptionID },
                exception);
        }

        // PUT: api/ClinicScheduleExceptions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateException(
            int id,
            ClinicScheduleExceptionDto dto)
        {
            var exception = await _context.ClinicScheduleExceptions
                .FirstOrDefaultAsync(e => e.ExceptionID == id);

            if (exception == null)
                return NotFound(new { message = "Schedule exception not found." });

            exception.ExceptionDate = dto.ExceptionDate.Date;
            exception.IsOpen = dto.IsOpen;
            exception.OpeningTime = dto.OpeningTime;
            exception.ClosingTime = dto.ClosingTime;
            exception.QueueCutoffTime = dto.QueueCutoffTime;
            exception.Reason = dto.Reason;
            exception.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(exception);
        }

        // DELETE: api/ClinicScheduleExceptions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteException(int id)
        {
            var exception = await _context.ClinicScheduleExceptions
                .FirstOrDefaultAsync(e => e.ExceptionID == id);

            if (exception == null)
                return NotFound(new { message = "Schedule exception not found." });

            exception.IsActive = false;
            exception.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Schedule exception deactivated successfully."
            });
        }
    }
}