// Models/CreateVaccinationDto.cs
namespace AndroidWebAPI.Models
{
    public class CreateVaccinationDto
    {
        public Guid ChildID { get; set; }
        public int VaccineID { get; set; }
        public int DoseNumber { get; set; }

        // String from HTML date input — controller does DateTime.Parse()
        public string DateAdministered { get; set; } = string.Empty;

        public string? LotNumber { get; set; }
        public string? Remarks { get; set; }

        // Guid? — Axios sends UUID string, ASP.NET deserializes automatically
        public Guid? AdministeredBy { get; set; }
        public string? AdministeredByName { get; set; }

        // Not sent from Vue, controller sets null explicitly — still needed for binding
        public DateTime? ScheduledDate { get; set; }
    }
}