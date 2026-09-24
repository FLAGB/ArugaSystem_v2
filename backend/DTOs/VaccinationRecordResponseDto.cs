using System;

namespace AndroidWebAPI.DTOs
{
    public class VaccinationRecordResponseDto
    {
        public Guid VaccinationRecordID { get; set; }

        public string RecordCode { get; set; } = string.Empty;

        public Guid ChildID { get; set; }

        public int VaccineID { get; set; }

        public string VaccineName { get; set; } = string.Empty;

        public int DoseNumber { get; set; }

        public DateTime VaccinationDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public Guid? AdministeredByUserID { get; set; }

        public string? AdministeredByName { get; set; }

        public string? NurseObservation { get; set; }

        public string? DoctorDiagnosis { get; set; }

        public Guid? DoctorDiagnosedByUserID { get; set; }

        public string? DoctorDiagnosedByName { get; set; }

        public DateTime? DoctorDiagnosedAt { get; set; }

        public string? LotNumber { get; set; }
    }
}