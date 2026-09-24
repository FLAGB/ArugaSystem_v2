using System;

namespace AndroidWebAPI.DTOs
{
    public class RecordVaccinationDto
    {
        public Guid ChildID { get; set; }

        public int VaccineID { get; set; }

        public int DoseNumber { get; set; }

        public DateTime VaccinationDate { get; set; }

        public int? InventoryID { get; set; }

        public Guid? AdministeredByUserID { get; set; }

        public string? NurseObservation { get; set; }

        public string? DoctorDiagnosis { get; set; }

        public Guid? DoctorDiagnosedByUserID { get; set; }

        public DateTime? DoctorDiagnosedAt { get; set; }
    }
}