using System;
using System.Collections.Generic;

namespace AndroidWebAPI.DTOs
{
    public class HistoricalVaccinationSubmissionDto
    {
        public Guid ChildID { get; set; }

        public List<HistoricalVaccinationItemDto> Vaccinations { get; set; } = new();
    }

    public class HistoricalVaccinationItemDto
    {
        public int VaccineID { get; set; }

        public int DoseNumber { get; set; }

        public DateTime VaccinationDate { get; set; }
    }
}