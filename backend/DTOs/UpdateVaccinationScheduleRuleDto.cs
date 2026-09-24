namespace AndroidWebAPI.DTOs
{
  
public class UpdateVaccinationScheduleRuleDto
{
    public int RuleID { get; set; }
    public int VaccineID { get; set; }
    public int DoseNumber { get; set; }
    public int MinimumAgeDays { get; set; }
    public int RecommendedAgeDays { get; set; }
    public int IntervalFromPreviousDoseDays { get; set; }
    public int SequenceOrder { get; set; }
    public bool IsRequired { get; set; }
}  
}
