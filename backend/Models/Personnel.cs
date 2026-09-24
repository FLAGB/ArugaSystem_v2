using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AndroidWebAPI.Models
{
    [Table("Personnel")]
    public class Personnel
    {
        [Key]
        public Guid PersonnelID { get; set; }

        public string PersonnelCode { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string? MiddleName { get; set; }

        public string LastName { get; set; } = string.Empty;

        // Doctor / Nurse / Midwife / Staff / Admin
        public string Role { get; set; } = string.Empty;

        public string? LicenseNumber { get; set; }

        public string Email { get; set; } = string.Empty;

        public string? ContactNo { get; set; }

        public string? Address { get; set; }

        public bool Status { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Navigation Property
        public virtual ICollection<VaccinationRecord> VaccinationRecords { get; set; }
            = new List<VaccinationRecord>();
    }
}