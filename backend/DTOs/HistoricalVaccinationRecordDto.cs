namespace AndroidWebAPI.DTOs
{
    public class HistoricalVaccinationRecordDto
{
    public Guid ChildID { get; set; }
    public int VaccineID { get; set; }
    public int DoseNumber { get; set; }
    public DateTime VaccinationDate { get; set; }
}
}