using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AndroidWebAPI.Models
{
    [Table("ClinicOperatingSchedule")]
    public class ClinicOperatingSchedule
    {
        [Key]
        public int ScheduleID { get; set; }

        // 0 = Sunday
        // 1 = Monday
        // 2 = Tuesday
        // 3 = Wednesday
        // 4 = Thursday
        // 5 = Friday
        // 6 = Saturday
        public int DayOfWeek { get; set; }

        public bool IsOpen { get; set; }

        public TimeSpan OpeningTime { get; set; }

        public TimeSpan ClosingTime { get; set; }

        // Last time a patient can enter the vaccination queue
        public TimeSpan QueueCutoffTime { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}