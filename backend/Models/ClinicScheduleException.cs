using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AndroidWebAPI.Models
{
    [Table("ClinicScheduleExceptions")]
    public class ClinicScheduleException
    {
        [Key]
        public int ExceptionID { get; set; }

        public DateTime ExceptionDate { get; set; }

        public bool IsOpen { get; set; }

        public TimeSpan OpeningTime { get; set; }

        public TimeSpan ClosingTime { get; set; }

        public TimeSpan QueueCutoffTime { get; set; }

        public string? Reason { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}