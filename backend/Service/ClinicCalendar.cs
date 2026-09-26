using AndroidWebAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace AndroidWebAPI.Services
{
    // Shared "when is the clinic next open?" lookup, used both when a
    // child's timeline is first generated and when future doses are
    // recalculated after a vaccination is recorded.
    public static class ClinicCalendar
    {
        // Returns `date` itself if the clinic is open that day, otherwise the
        // next open day. A ClinicScheduleExceptions row (holiday / special
        // opening) overrides the normal weekly ClinicOperatingSchedule.
        public static async Task<DateTime> NextOpenDayAsync(AppDbContext context, DateTime date)
        {
            date = date.Date;

            var exceptions = await context.ClinicScheduleExceptions
                .Where(e => e.IsActive && e.ExceptionDate >= date && e.ExceptionDate < date.AddDays(366))
                .ToListAsync();

            var weekly = await context.ClinicOperatingSchedules
                .Where(s => s.IsActive)
                .ToListAsync();

            // No operating schedule configured at all — don't loop forever,
            // just keep the date as-is.
            if (!weekly.Any(s => s.IsOpen) && !exceptions.Any(e => e.IsOpen))
                return date;

            for (int i = 0; i < 366; i++)
            {
                var exception = exceptions.FirstOrDefault(e => e.ExceptionDate.Date == date);

                if (exception != null)
                {
                    if (exception.IsOpen) return date;
                }
                else
                {
                    var day = weekly.FirstOrDefault(s => s.DayOfWeek == (int)date.DayOfWeek);
                    if (day != null && day.IsOpen) return date;
                }

                date = date.AddDays(1);
            }

            throw new Exception("No open clinic day found within the next year.");
        }

        // "Mon–Fri, 8:00 AM – 5:00 PM", built from the weekly schedule the
        // admin sets under Operating Hours, for messages sent to parents.
        public static async Task<string> DescribeHoursAsync(AppDbContext context)
        {
            var open = (await context.ClinicOperatingSchedules
                    .Where(s => s.IsActive && s.IsOpen)
                    .ToListAsync())
                .OrderBy(s => (s.DayOfWeek + 6) % 7)   // Monday first
                .ToList();

            if (open.Count == 0) return "during clinic hours";

            string[] names = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
            var parts = new List<string>();
            int i = 0;
            while (i < open.Count)
            {
                int j = i;
                while (j + 1 < open.Count &&
                       (open[j + 1].DayOfWeek + 6) % 7 == (open[j].DayOfWeek + 6) % 7 + 1)
                    j++;
                // 3+ days in a row read better as a range
                if (j - i >= 2) parts.Add($"{names[open[i].DayOfWeek]}–{names[open[j].DayOfWeek]}");
                else for (int k = i; k <= j; k++) parts.Add(names[open[k].DayOfWeek]);
                i = j + 1;
            }

            var from = open.Min(s => s.OpeningTime);
            var to = open.Max(s => s.ClosingTime);
            return $"{string.Join(", ", parts)}, {Time(from)} – {Time(to)}";

            static string Time(TimeSpan t) => DateTime.Today.Add(t).ToString("h:mm tt");
        }
    }
}
