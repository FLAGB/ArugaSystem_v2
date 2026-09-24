using AndroidWebAPI.Models;

namespace AndroidWebAPI.Repositories
{
    public interface IClinicOperatingScheduleRepository
    {
        Task<ClinicOperatingSchedule?> GetByDayAsync(int dayOfWeek);
        Task<ClinicScheduleException?> GetExceptionAsync(DateTime date);
    }
}