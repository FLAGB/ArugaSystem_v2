namespace AndroidWebAPI.DTOs
{
    // Body for PATCH /api/VaccinationRecords/{id}/diagnosis.
    // DoctorDiagnosis is nullable so a doctor can clear a diagnosis by
    // saving an empty field, same as the frontend's `|| null` behavior.
    public class UpdateDiagnosisDto
    {
        public string? DoctorDiagnosis { get; set; }
        public Guid? DiagnosedByUserID { get; set; }
    }
}