using AndroidWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using AndroidWebAPI.DTOs;

namespace AndroidWebAPI.Data
{
    public class VaccinationTimelineRepository : IVaccinationTimelineRepository
    {
        private readonly AppDbContext _context;

        public VaccinationTimelineRepository(AppDbContext context)
        {
            _context = context;
        }

        // ============================
        // CRUD
        // ============================

        public async Task<IEnumerable<VaccinationTimeline>> GetAllAsync()
        {
            return await _context.VaccinationTimelines
                .Include(t => t.Child)
                .Include(t => t.Vaccine)
                .ToListAsync();
        }

        public async Task<VaccinationTimeline?> GetByIdAsync(Guid timelineId)
        {
            return await _context.VaccinationTimelines
                .Include(t => t.Child)
                .Include(t => t.Vaccine)
                .FirstOrDefaultAsync(t => t.TimelineID == timelineId);
        }
public async Task<IEnumerable<VaccinationTimeline>> GetByChildAsync(Guid childId)
{
    return await _context.VaccinationTimelines
        .Where(t => t.ChildID == childId)
        .Include(t => t.Child)
        .Include(t => t.Vaccine)
        .OrderBy(t => t.ExpectedDate)
        .ToListAsync();
}
        public async Task AddAsync(VaccinationTimeline timeline)
        {
            await _context.VaccinationTimelines.AddAsync(timeline);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(VaccinationTimeline timeline)
        {
            _context.VaccinationTimelines.Update(timeline);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid timelineId)
        {
            var timeline = await _context.VaccinationTimelines.FindAsync(timelineId);

            if (timeline != null)
            {
                _context.VaccinationTimelines.Remove(timeline);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid childId, int vaccineId, int doseNumber)
        {
            return await _context.VaccinationTimelines.AnyAsync(t =>
                t.ChildID == childId &&
                t.VaccineID == vaccineId &&
                t.DoseNumber == doseNumber);
        }

        // ============================
        // BUSINESS LOGIC
        // ============================
        // 
        public async Task GenerateTimelineAsync(Guid childId)
{
    // 1. Get child
    var child = await _context.Children
        .FirstOrDefaultAsync(c => c.ChildID == childId);

    if (child == null)
        throw new Exception("Child not found.");

    // 2. Prevent duplicate timeline generation
    bool timelineExists = await _context.VaccinationTimelines
        .AnyAsync(t =>
            t.ChildID == childId &&
            t.Status == "Pending");

    if (timelineExists)
        throw new Exception(
            "Vaccination timeline already exists for this child.");

    // 3. Load vaccination schedule rules
    var rules = await _context.VaccinationScheduleRules
        .OrderBy(r => r.SequenceOrder)
        .ToListAsync();

    if (!rules.Any())
        throw new Exception(
            "No vaccination schedule rules found.");

    // 4. Load existing vaccination records
    var vaccinationRecords = await _context.VaccinationRecords
        .Where(r => r.ChildID == childId)
        .ToListAsync();

    // 5. Load existing timelines
    var existingTimelines = await _context.VaccinationTimelines
        .Where(t => t.ChildID == childId)
        .ToListAsync();

    // 6. Generate only missing timelines
    var timelines = new List<VaccinationTimeline>();

    foreach (var rule in rules)
    {
        var existingTimeline = existingTimelines
            .FirstOrDefault(t =>
                t.VaccineID == rule.VaccineID &&
                t.DoseNumber == rule.DoseNumber);

        if (existingTimeline != null)
            continue;

        var existingRecord = vaccinationRecords
            .FirstOrDefault(r =>
                r.VaccineID == rule.VaccineID &&
                r.DoseNumber == rule.DoseNumber &&
                r.Status == "Completed");

        var expectedDate =
            child.BirthDate.AddDays(rule.RecommendedAgeDays);

        var scheduledDate = await GetNextAvailableVaccinationDateAsync(expectedDate);

var timeline = new VaccinationTimeline
{
    TimelineID = Guid.NewGuid(),

    TimelineCode =
        $"TL-{Guid.NewGuid().ToString("N")[..12].ToUpper()}",

    ChildID = child.ChildID,
    VaccineID = rule.VaccineID,
    DoseNumber = rule.DoseNumber,

    ExpectedDate = expectedDate,
    ScheduledDate = scheduledDate,

    CreatedAt = DateTime.UtcNow
};

        if (existingRecord != null)
        {
            timeline.Status = "Completed";
            timeline.VaccinationRecordID =
                existingRecord.VaccinationRecordID;
        }
        else
        {
            timeline.Status = "Pending";
            timeline.VaccinationRecordID = null;
        }

        timelines.Add(timeline);
    }

    // 7. Save
    if (timelines.Any())
    {
        await _context.VaccinationTimelines
            .AddRangeAsync(timelines);

        await _context.SaveChangesAsync();
    }
}

private async Task<DateTime> GetNextAvailableVaccinationDateAsync(
    DateTime expectedDate)
{
    var date = expectedDate.Date;

    // Safety limit so we don't accidentally loop forever
    for (int i = 0; i < 365; i++)
    {
        // 1. Check if there is a special exception for this date
        var exception = await _context.ClinicScheduleExceptions
            .FirstOrDefaultAsync(e =>
                e.ExceptionDate.Date == date &&
                e.IsActive);

        if (exception != null)
        {
            // Exception overrides the normal weekly schedule
            if (exception.IsOpen)
            {
                return date;
            }

            // Exception says clinic is closed
            date = date.AddDays(1);
            continue;
        }

        // 2. No exception, so check normal weekly schedule
        int dayOfWeek = (int)date.DayOfWeek;

        var schedule = await _context.ClinicOperatingSchedules
            .FirstOrDefaultAsync(s =>
                s.DayOfWeek == dayOfWeek &&
                s.IsActive);

        // 3. Normal schedule says clinic is open
        if (schedule != null && schedule.IsOpen)
        {
            return date;
        }

        // 4. Closed normally → try the next day
        date = date.AddDays(1);
    }

    throw new Exception(
        "No available pediatric vaccination schedule found within the next 365 days.");
}
                public async Task MarkCompletedAsync(Guid timelineId)
{
    var timeline = await _context.VaccinationTimelines
        .FirstOrDefaultAsync(t => t.TimelineID == timelineId);

    if (timeline == null)
        throw new Exception("Vaccination timeline not found.");

    timeline.Status = "Completed";
    timeline.UpdatedAt = DateTime.Now;

    _context.VaccinationTimelines.Update(timeline);
    await _context.SaveChangesAsync();
}
public async Task<IEnumerable<VaccinationTimeline>> GetDueTodayAsync()
{
    var today = DateTime.Today;

    return await _context.VaccinationTimelines
        .Include(t => t.Child)
            .ThenInclude(c => c.ParentRelationships)
            .ThenInclude(pr => pr.Parent)
        .Include(t => t.Vaccine)
        .Where(t =>
            t.Status == "Pending" &&
            t.ScheduledDate.Date == today)
        .OrderBy(t => t.ScheduledDate)
        .ToListAsync();
}

public async Task<IEnumerable<VaccinationTimeline>> GetUpcomingAsync(int days)
{
    var today = DateTime.Today;
    var endDate = today.AddDays(days);

    return await _context.VaccinationTimelines
        .Include(t => t.Child)
        .Include(t => t.Vaccine)
        .Where(t =>
            t.Status == "Pending" &&
            t.ScheduledDate.Date >= today &&
            t.ScheduledDate.Date <= endDate)
        .OrderBy(t => t.ScheduledDate)
        .ToListAsync();
}

public async Task RegenerateTimelineAsync(Guid childId)
{
    // Get the child
    var child = await _context.Children
        .FirstOrDefaultAsync(c => c.ChildID == childId);

    if (child == null)
        throw new Exception("Child not found.");

    // Delete only pending timeline records
    var pendingTimelines = await _context.VaccinationTimelines
        .Where(t => t.ChildID == childId &&
                    t.Status == "Pending")
        .ToListAsync();

    _context.VaccinationTimelines.RemoveRange(pendingTimelines);
    await _context.SaveChangesAsync();

    // Generate a new timeline
    await GenerateTimelineAsync(childId);
}
public async Task<TimelineSummaryDto> GetTimelineSummaryAsync(Guid childId)
{
    var timelines = await _context.VaccinationTimelines
        .Where(t => t.ChildID == childId)
        .ToListAsync();

    return new TimelineSummaryDto
    {
        ChildID = childId,
        HasTimeline = timelines.Any(),
        Pending = timelines.Count(t => t.Status == "Pending"),
        Completed = timelines.Count(t => t.Status == "Completed"),
        Missed = timelines.Count(t => t.Status == "Missed")
    };
}

public async Task UpdateMissedVaccinationsAsync()
{
    var today = DateTime.Today;

    var overdueTimelines = await _context.VaccinationTimelines
        .Where(t =>
            t.Status == "Pending" &&
            t.ScheduledDate.Date < today)
        .ToListAsync();

    foreach (var timeline in overdueTimelines)
    {
        timeline.Status = "Missed";
        timeline.UpdatedAt = DateTime.UtcNow;
    }

    if (overdueTimelines.Any())
    {
        await _context.SaveChangesAsync();
    }
}
    }
}