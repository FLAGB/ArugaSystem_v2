using AndroidWebAPI.Models;
using AndroidWebAPI.DTOs;

namespace AndroidWebAPI.Data
{
    public interface IVaccinationTimelineRepository
    {
        // CRUD
        Task<IEnumerable<VaccinationTimeline>> GetAllAsync();
        Task<VaccinationTimeline?> GetByIdAsync(Guid timelineId);
        Task<IEnumerable<VaccinationTimeline>> GetByChildAsync(Guid childId);

        Task AddAsync(VaccinationTimeline timeline);
        Task UpdateAsync(VaccinationTimeline timeline);
        Task DeleteAsync(Guid timelineId);

        Task<bool> ExistsAsync(Guid childId, int vaccineId, int doseNumber);

        // Business Logic (Current Sprint)
        Task GenerateTimelineAsync(Guid childId);

        Task MarkCompletedAsync(Guid timelineId);

        Task RegenerateTimelineAsync(Guid childId);
        Task UpdateMissedVaccinationsAsync();

        Task<IEnumerable<VaccinationTimeline>> GetDueTodayAsync();

        Task<IEnumerable<VaccinationTimeline>> GetUpcomingAsync(int days);
        Task<TimelineSummaryDto> GetTimelineSummaryAsync(Guid childId);
    }
}   