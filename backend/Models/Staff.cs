using System.ComponentModel.DataAnnotations;

namespace AndroidWebAPI.Models
{
    public class Staff
    {
        [Key]
        public Guid StaffID { get; set; }

        public Guid AccountID { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string? MiddleName { get; set; }

        public string LastName { get; set; } = string.Empty;

        public string ContactNo { get; set; } = string.Empty;

        public string? Address { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }
    }
}