using AndroidWebAPI.Data;
using AndroidWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AndroidWebAPI.Repositories
{
    public class ClinicOperatingScheduleRepository
        : IClinicOperatingScheduleRepository
    {
        private readonly AppDbContext _context;

        public ClinicOperatingScheduleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ClinicOperatingSchedule?> GetByDayAsync(
            int dayOfWeek)
        {
            return await _context.ClinicOperatingSchedules
                .FirstOrDefaultAsync(s =>
                    s.DayOfWeek == dayOfWeek &&
                    s.IsActive);
        }

        public async Task<ClinicScheduleException?> GetExceptionAsync(
            DateTime date)
        {
            return await _context.ClinicScheduleExceptions
                .FirstOrDefaultAsync(e =>
                    e.ExceptionDate.Date == date.Date &&
                    e.IsActive);
        }
    }
}