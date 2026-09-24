using AndroidWebAPI.Models;

namespace AndroidWebAPI.Data
{
    public interface IVaccinationScheduleRuleRepository
    {
        // CRUD
        Task<IEnumerable<VaccinationScheduleRule>> GetAllAsync();

        Task<IEnumerable<VaccinationScheduleRule>> GetByVaccineAsync(int vaccineId);

        Task<VaccinationScheduleRule?> GetRuleAsync(int vaccineId, int doseNumber);

        Task AddAsync(VaccinationScheduleRule rule);

        Task UpdateAsync(VaccinationScheduleRule rule);

        Task DeleteAsync(int ruleId);

        // Business Logic
        Task<bool> ExistsAsync(int vaccineId, int doseNumber);

        Task<IEnumerable<VaccinationScheduleRule>> GetScheduleAsync();
    }
}