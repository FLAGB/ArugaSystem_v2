// Services/NotificationGeneratorService.cs
//
// Register in Program.cs with:
//   builder.Services.AddHostedService<NotificationGeneratorService>();
//
// This runs once per day at startup + every 24 hours.
// PRE-DUE:  sends reminders at 30, 7, 5, and 1 day before each scheduled dose.
// POST-DUE: sends overdue follow-ups at 1, 5, 14, and 30 days after a missed dose.
// STOCK:    every Wednesday checks VaccineInventory and alerts affected parents.

using AndroidWebAPI.Data;   // ← This is what's missing — AppDbContext lives here
using AndroidWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class NotificationGeneratorService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<NotificationGeneratorService> _logger;

    // Days BEFORE due date to send pre-reminders
    private static readonly int[] PreReminders  = { 30, 7, 5, 1 };

    // Days AFTER due date to send overdue follow-ups
    // 1 day → gentle miss, 5 days → overdue, 14 days → protection gap, 30 days → urgent
    private static readonly int[] PostReminders = { 1, 5, 14, 30 };

    // Low stock threshold — alert parents when stock drops to or below this
    private const int LowStockThreshold = 5;

    public NotificationGeneratorService(
        IServiceScopeFactory scopeFactory,
        ILogger<NotificationGeneratorService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Run immediately on startup, then every 24 hours
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await GenerateNotificationsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating notifications");
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

    private async Task GenerateNotificationsAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var today = DateOnly.FromDateTime(DateTime.Today);

        // ── 1. VACCINE REMINDERS ─────────────────────────────────────────────
        //
        // For each child, compute their upcoming dose dates using the same
        // MinIntervalDays logic as the frontend, then create notifications
        // at 30/7/5/1 days before each due date — but only if a notification
        // of that type doesn't already exist for that child+vaccine+dose.

var children = await context.Children
    .Include(c => c.ParentRelationships)
    .ToListAsync();


        var vaccineDoses = await context.VaccineDoses
        .Include(d => d.Vaccine)
        .OrderBy(d => d.VaccineID).ThenBy(d => d.DoseNumber)
        .ToListAsync();

        var existingRecords = await context.VaccinationRecords.ToListAsync();
        var existingNotifications = await context.Notifications.ToListAsync();

        foreach (var child in children)
        {
            var primaryParent = child.ParentRelationships
    .FirstOrDefault(r =>
        r.IsPrimaryContact &&
        r.Status == "Active");

if (primaryParent == null)
    continue;

            var schedule  = ComputeSchedule(child, vaccineDoses, existingRecords);
            string childName = $"{child.FirstName} {child.LastName}";

            foreach (var entry in schedule)
            {
                bool isAdministered = existingRecords.Any(r =>
                    r.ChildID    == child.ChildID    &&
                    r.VaccineID  == entry.VaccineID  &&
                    r.DoseNumber == entry.DoseNumber &&
                    r.Status     == "Completed");

                if (isAdministered) continue;

                string doseLabel = $"Dose {entry.DoseNumber}";
                string dateLabel = entry.ScheduledDate.ToString("MMMM d, yyyy");

                // ── PRE-DUE REMINDERS ────────────────────────────────────────
                foreach (int daysBefore in PreReminders)
                {
                    var triggerDate = DateOnly.FromDateTime(entry.ScheduledDate.AddDays(-daysBefore));
                    if (triggerDate > today) continue;

                    string type = daysBefore switch
                    {
                        30 => "ReminderMonth",
                        7  => "ReminderWeek",
                        5  => "Reminder5Day",
                        1  => "ReminderDay",
                        _  => "Reminder"
                    };

                    if (existingNotifications.Any(n =>
                            n.ParentID == primaryParent.ParentID && n.ChildID == child.ChildID &&
                            n.VaccineID == entry.VaccineID && n.DoseNumber == entry.DoseNumber &&
                            n.Type == type)) continue;

                    string timeLabel = daysBefore switch
                    {
                        30 => "in 1 month",
                        7  => "in 1 week",
                        5  => "in 5 days",
                        1  => "tomorrow",
                        _  => $"in {daysBefore} days"
                    };

                    // Tone escalates as the date approaches
                    string message = daysBefore switch
                    {
                        30 => $"Heads up! {entry.VaccineName} ({doseLabel}) for {childName} is coming up on {dateLabel}. " +
                              $"Plan a visit to Leveriza Health Center on Mon, Wed, or Fri · 8:00 AM – 12:00 PM.",
                        7  => $"{entry.VaccineName} ({doseLabel}) for {childName} is due on {dateLabel} — that's one week away. " +
                              $"Make sure to visit Leveriza Health Center on a Mon, Wed, or Fri · 8:00 AM – 12:00 PM.",
                        5  => $"Don't forget! {entry.VaccineName} ({doseLabel}) for {childName} is due on {dateLabel}. " +
                              $"Only 5 days left. Visit Leveriza Health Center on Mon, Wed, or Fri · 8:00 AM – 12:00 PM.",
                        1  => $"Reminder: {entry.VaccineName} ({doseLabel}) for {childName} is scheduled TOMORROW, {dateLabel}. " +
                              $"Please visit Leveriza Health Center between 8:00 AM and 12:00 PM.",
                        _  => $"{entry.VaccineName} ({doseLabel}) for {childName} is due on {dateLabel}."
                    };

                    var notif = new Notification
                    {
                        ParentID = primaryParent.ParentID,
                        ChildID       = child.ChildID,
                        VaccineID     = entry.VaccineID,
                        DoseNumber    = entry.DoseNumber,
                        Type          = type,
                        Title         = $"Vaccine due {timeLabel} — {childName}",
                        Message       = message,
                        ScheduledDate = entry.ScheduledDate,
                        IsRead        = false,
                        CreatedAt     = DateTime.Now
                    };
                    context.Notifications.Add(notif);
                    existingNotifications.Add(notif);
                }

                // ── POST-DUE OVERDUE FOLLOW-UPS ──────────────────────────────
                // Only fire if the scheduled date has already passed
                if (DateOnly.FromDateTime(entry.ScheduledDate) >= today) continue;

                foreach (int daysAfter in PostReminders)
                {
                    var triggerDate = DateOnly.FromDateTime(entry.ScheduledDate.AddDays(daysAfter));
                    if (triggerDate > today) continue;

                    string type = daysAfter switch
                    {
                        1  => "OverdueMiss",       // gentle — missed yesterday
                        5  => "Overdue5Day",        // overdue — act this week
                        14 => "Overdue2Week",       // protection gap forming
                        30 => "OverdueUrgent",      // urgent — 1 month lapsed
                        _  => "Overdue"
                    };

                    if (existingNotifications.Any(n =>
                            n.ParentID == primaryParent.ParentID && n.ChildID == child.ChildID &&
                            n.VaccineID == entry.VaccineID && n.DoseNumber == entry.DoseNumber &&
                            n.Type == type)) continue;

                    // Urgency escalates with each post-due interval
                    (string title, string message) = daysAfter switch
                    {
                        1 => (
                            $"Missed vaccine — {childName}",
                            $"{entry.VaccineName} ({doseLabel}) was scheduled yesterday, {dateLabel}. " +
                            $"Please visit Leveriza Health Center as soon as possible on the next available Mon, Wed, or Fri · 8:00 AM – 12:00 PM."
                        ),
                        5 => (
                            $"Vaccine overdue — {childName}",
                            $"{entry.VaccineName} ({doseLabel}) for {childName} is now 5 days overdue (was due {dateLabel}). " +
                            $"Please visit Leveriza Health Center this week to keep {childName.Split(' ')[0]} protected."
                        ),
                        14 => (
                            $"2 weeks overdue — {childName}",
                            $"{entry.VaccineName} ({doseLabel}) is 2 weeks overdue for {childName}. " +
                            $"A delay this long may leave your child unprotected. Please visit Leveriza Health Center urgently on Mon, Wed, or Fri · 8:00 AM – 12:00 PM."
                        ),
                        30 => (
                            $"URGENT: 1 month overdue — {childName}",
                            $"{entry.VaccineName} ({doseLabel}) for {childName} is now 1 month overdue. " +
                            $"Immediate vaccination is strongly recommended. Please visit Leveriza Health Center on the nearest Mon, Wed, or Fri · 8:00 AM – 12:00 PM."
                        ),
                        _ => (
                            $"Vaccine overdue — {childName}",
                            $"{entry.VaccineName} ({doseLabel}) is overdue for {childName}."
                        )
                    };

                    var notif = new Notification
                    {
                        ParentID = primaryParent.ParentID,
                        ChildID       = child.ChildID,
                        VaccineID     = entry.VaccineID,
                        DoseNumber    = entry.DoseNumber,
                        Type          = type,
                        Title         = title,
                        Message       = message,
                        ScheduledDate = entry.ScheduledDate,
                        IsRead        = false,
                        CreatedAt     = DateTime.Now
                    };
                    context.Notifications.Add(notif);
                    existingNotifications.Add(notif);
                    
                }
            }
        }
    await context.SaveChangesAsync();
        _logger.LogInformation("Notifications generated at {Time}", DateTime.Now);
    }


    // ── Schedule computation (mirrors frontend VACCINE_MASTER logic) ──────────
    private static List<ScheduleEntry> ComputeSchedule(
    Child child,
    List<VaccineDose> allDoses,
    List<VaccinationRecord> records)
{
    var result     = new List<ScheduleEntry>();
    var vaccineIds = allDoses.Select(d => d.VaccineID).Distinct().OrderBy(x => x);
    var birth      = child.BirthDate;

    foreach (var vaccineId in vaccineIds)
    {
        var doses    = allDoses.Where(d => d.VaccineID == vaccineId)
                               .OrderBy(d => d.DoseNumber).ToList();
        DateTime? prevDate = null;

        foreach (var dose in doses)
        {
            // ✅ Check if this dose was actually administered
            var administered = records.FirstOrDefault(r =>
                r.ChildID    == child.ChildID   &&
                r.VaccineID  == vaccineId        &&
                r.DoseNumber == dose.DoseNumber  &&
                r.Status     == "Completed");

            DateTime scheduled;

            if (administered?.VaccinationDate != null)
            {
                // ✅ Use ACTUAL date — cascades correctly to next dose
                scheduled = administered.VaccinationDate;
            }
            else
            {
                // Not yet taken — compute from last known date
                DateTime baseDate = dose.DoseNumber == 1
                    ? birth.AddDays(dose.MinIntervalDays)
                    : (prevDate ?? birth).AddDays(dose.MinIntervalDays);

                scheduled = SnapToClinicDay(baseDate);
            }

            // ✅ This is the key — prevDate carries forward the actual date
            prevDate = scheduled;

            result.Add(new ScheduleEntry
            {
                VaccineID     = vaccineId,
                VaccineName   = doses[0].Vaccine?.VaccineName ?? $"Vaccine {vaccineId}",
                DoseNumber    = dose.DoseNumber,
                ScheduledDate = scheduled
            });
        }
    }

    return result;
}

    private static DateTime SnapToClinicDay(DateTime date)
    {
        while (date.DayOfWeek != DayOfWeek.Monday &&
               date.DayOfWeek != DayOfWeek.Wednesday &&
               date.DayOfWeek != DayOfWeek.Friday)
        {
            date = date.AddDays(1);
        }
        return date;
    }

    private record ScheduleEntry
    {
        public int      VaccineID     { get; init; }
        public string   VaccineName   { get; init; } = string.Empty;
        public int      DoseNumber    { get; init; }
        public DateTime ScheduledDate { get; init; }
    }
}
