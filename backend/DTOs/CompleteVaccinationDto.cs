using System;

namespace AndroidWebAPI.DTOs
{
    public class CompleteVaccinationDto
    {
        public int InventoryID { get; set; }
        public Guid AdministeredByUserID { get; set; }
        public DateTime VaccinationDate { get; set; }
        public string? NurseObservation { get; set; }
    }
}