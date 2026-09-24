using AndroidWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AndroidWebAPI.Data.Repositories
{
    public class VaccinationScheduleRuleRepository : IVaccinationScheduleRuleRepository
    {
        private readonly AppDbContext _context;

        public VaccinationScheduleRuleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VaccinationScheduleRule>> GetAllAsync()
        {
            return await _context.VaccinationScheduleRules
                .OrderBy(r => r.SequenceOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<VaccinationScheduleRule>> GetByVaccineAsync(int vaccineId)
        {
            return await _context.VaccinationScheduleRules
                .Where(r => r.VaccineID == vaccineId)
                .OrderBy(r => r.DoseNumber)
                .ToListAsync();
        }

        public async Task<VaccinationScheduleRule?> GetRuleAsync(int vaccineId, int doseNumber)
        {
            return await _context.VaccinationScheduleRules
                .FirstOrDefaultAsync(r =>
                    r.VaccineID == vaccineId &&
                    r.DoseNumber == doseNumber);
        }

        public async Task AddAsync(VaccinationScheduleRule rule)
        {
            await _context.VaccinationScheduleRules.AddAsync(rule);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(VaccinationScheduleRule rule)
        {
            _context.VaccinationScheduleRules.Update(rule);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int ruleId)
        {
            var rule = await _context.VaccinationScheduleRules.FindAsync(ruleId);

            if (rule != null)
            {
                _context.VaccinationScheduleRules.Remove(rule);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<bool> ExistsAsync(int vaccineId, int doseNumber)
{
    return await _context.VaccinationScheduleRules
        .AnyAsync(r =>
            r.VaccineID == vaccineId &&
            r.DoseNumber == doseNumber);
}

public async Task<IEnumerable<VaccinationScheduleRule>> GetScheduleAsync()
{
    return await _context.VaccinationScheduleRules
        .Include(r => r.Vaccine)
        .OrderBy(r => r.SequenceOrder)
        .ThenBy(r => r.DoseNumber)
        .ToListAsync();
}
    }
}