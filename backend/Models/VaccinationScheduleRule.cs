using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AndroidWebAPI.Models
{
    [Table("VaccinationScheduleRules")]
    public class VaccinationScheduleRule
    {
        [Key]
        public int RuleID { get; set; }

        public int VaccineID { get; set; }

        public int DoseNumber { get; set; }

        // Age from birth (days)
        public int MinimumAgeDays { get; set; }

        // Recommended age from birth (days)
        public int RecommendedAgeDays { get; set; }

        // Days after previous dose
        public int IntervalFromPreviousDoseDays { get; set; }

        // Order of administration
        public int SequenceOrder { get; set; }

        public bool IsRequired { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        //------------------------------------
        // Navigation Property
        //------------------------------------

        [ForeignKey(nameof(VaccineID))]
        public virtual Vaccine? Vaccine { get; set; }
    }
}