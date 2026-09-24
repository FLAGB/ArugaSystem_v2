// Models/Vaccine.cs
// Maps to dbo.Vaccines — columns: VaccineID, VaccineName, Description
// NOTE: MinIntervalDays is in dbo.VaccineDoses, NOT here.
using System.ComponentModel.DataAnnotations;

namespace AndroidWebAPI.Models
{
    public class Vaccine
    {
        [Key]
       public int VaccineID { get; set; }
public required string VaccineName { get; set; }
public string? Abbreviation { get; set; }
public string? Description { get; set; }
public string? AgeCategory { get; set; }
public string? TargetDisease { get; set; }
public string? RecommendedAge { get; set; }
public int NumberOfRequiredDoses { get; set; }
public string? DoseInterval { get; set; }
public string? AdministrationRoute { get; set; }
public bool Status { get; set; }
public DateTime CreatedAt { get; set; }
public DateTime? UpdatedAt { get; set; }
    }
}