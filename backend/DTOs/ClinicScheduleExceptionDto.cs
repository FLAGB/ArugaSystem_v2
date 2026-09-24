using System;

namespace AndroidWebAPI.DTOs
{
    public class ClinicScheduleExceptionDto
    {
        public DateTime ExceptionDate { get; set; }

        public bool IsOpen { get; set; }

        public TimeSpan OpeningTime { get; set; }

        public TimeSpan ClosingTime { get; set; }

        public TimeSpan QueueCutoffTime { get; set; }

        public string? Reason { get; set; }
    }
}