using AndroidWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using AndroidWebAPI.DTOs;
namespace AndroidWebAPI.Data.Repositories
{
    public class VaccinationRecordRepository : IVaccinationRecordRepository
    {
        private readonly AppDbContext _context;

        public VaccinationRecordRepository(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<VaccinationRecord>> GetAllAsync()
        {
            return await _context.VaccinationRecords
                .Include(r => r.Child)
                    .ThenInclude(c => c.ParentRelationships)
                    .ThenInclude(pr => pr.Parent)
                .Include(r => r.Vaccine)
                .Include(r => r.Inventory)
                .Include(r => r.AdministeredBy)
                .ToListAsync();
        }
     


        public async Task<VaccinationRecord?> GetByIdAsync(Guid vaccinationRecordId)
        {
            return await _context.VaccinationRecords
                .FirstOrDefaultAsync(r => r.VaccinationRecordID == vaccinationRecordId);
        }
        
public async Task<IEnumerable<VaccinationRecordResponseDto>> GetByChildAsync(Guid childId)
{
    return await _context.VaccinationRecords
        .Where(r => r.ChildID == childId)
        .OrderBy(r => r.VaccinationDate)
        .Select(r => new VaccinationRecordResponseDto
        {
            VaccinationRecordID = r.VaccinationRecordID,
            RecordCode = r.RecordCode,
            ChildID = r.ChildID,
            VaccineID = r.VaccineID,
            VaccineName = r.Vaccine != null
                ? r.Vaccine.VaccineName
                : string.Empty,
            DoseNumber = r.DoseNumber,
            VaccinationDate = r.VaccinationDate,
            Status = r.Status,
            AdministeredByUserID = r.AdministeredByUserID,
            AdministeredByName = r.AdministeredBy != null
                ? r.AdministeredBy.FirstName + " " + r.AdministeredBy.LastName
                : null,
            NurseObservation = r.NurseObservation,
            DoctorDiagnosis = r.DoctorDiagnosis,
            DoctorDiagnosedByUserID = r.DoctorDiagnosedByUserID,
            DoctorDiagnosedByName = r.DoctorDiagnosedBy != null
                ? r.DoctorDiagnosedBy.FirstName + " " + r.DoctorDiagnosedBy.LastName
                : null,
            DoctorDiagnosedAt = r.DoctorDiagnosedAt,
            LotNumber = r.Inventory != null ? r.Inventory.LotNumber : null
        })
        .ToListAsync();
}
        

public async Task<VaccinationRecord> CompleteVaccinationAsync(Guid vaccinationRecordId, CompleteVaccinationDto dto)
{
    await using var transaction = await _context.Database.BeginTransactionAsync();
    try
    {
        var record = await _context.VaccinationRecords
            .FirstOrDefaultAsync(r => r.VaccinationRecordID == vaccinationRecordId);

        if (record == null)
            throw new Exception("Vaccination record not found.");

        if (record.Status == "Completed")
            throw new Exception("This vaccination has already been completed.");

        var inventory = await _context.VaccineInventory
            .FirstOrDefaultAsync(i => i.InventoryID == dto.InventoryID);

        if (inventory == null)
            throw new Exception("Vaccine inventory not found.");

        if (inventory.VaccineID != record.VaccineID)
            throw new Exception("The selected inventory does not belong to this record's vaccine.");

        if (!inventory.Status)
            throw new Exception("The selected vaccine inventory is inactive.");

        if (inventory.ExpirationDate.Date < DateTime.Today)
            throw new Exception("The selected vaccine inventory has expired.");

        if (inventory.CurrentQuantity <= 0)
            throw new Exception("No vaccine stock remaining for the selected batch.");

        inventory.CurrentQuantity--;
        inventory.UpdatedAt = DateTime.UtcNow;

        record.InventoryID = dto.InventoryID;
        record.AdministeredByUserID = dto.AdministeredByUserID;
        record.VaccinationDate = dto.VaccinationDate;
        record.NurseObservation = dto.NurseObservation;
        record.Status = "Completed";
        record.UpdatedAt = DateTime.UtcNow;

        var timeline = await _context.VaccinationTimelines
            .FirstOrDefaultAsync(t =>
                t.ChildID == record.ChildID &&
                t.VaccineID == record.VaccineID &&
                t.DoseNumber == record.DoseNumber &&
                t.Status == "Pending");

        if (timeline != null)
        {
            timeline.Status = "Completed";
            timeline.VaccinationRecordID = record.VaccinationRecordID;
            timeline.UpdatedAt = DateTime.UtcNow;
            record.TimelineID = timeline.TimelineID;
        }

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        return record;
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}

public async Task<VaccinationRecord> UpdateDiagnosisAsync(Guid vaccinationRecordId, UpdateDiagnosisDto dto)
{
    var record = await _context.VaccinationRecords
        .FirstOrDefaultAsync(r => r.VaccinationRecordID == vaccinationRecordId);

    if (record == null)
        throw new Exception("Vaccination record not found.");

    record.DoctorDiagnosis = dto.DoctorDiagnosis;
    record.DoctorDiagnosedByUserID = dto.DiagnosedByUserID;
    record.DoctorDiagnosedAt = DateTime.UtcNow;
    record.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();
    return record;
}

        public async Task AddAsync(VaccinationRecord record)
        {
            await _context.VaccinationRecords.AddAsync(record);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(VaccinationRecord record)
        {
            _context.VaccinationRecords.Update(record);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid vaccinationRecordId)
        {
            var record = await _context.VaccinationRecords.FindAsync(vaccinationRecordId);

            if (record != null)
            {
                _context.VaccinationRecords.Remove(record);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> AlreadyVaccinatedAsync(Guid childId, int vaccineId, int doseNumber)
        {
            // Only a row that was actually administered counts as "already
            // vaccinated" — a Scheduled/Overdue row for the same dose (e.g.
            // a duplicate timeline entry) must not block recording the dose.
            return await _context.VaccinationRecords.AnyAsync(r =>
                r.ChildID == childId &&
                r.VaccineID == vaccineId &&
                r.DoseNumber == doseNumber &&
                r.Status == "Completed");
        }
        
public async Task RecordVaccinationAsync(VaccinationRecord record)
{
    await using var transaction =
        await _context.Database.BeginTransactionAsync();

    try
    {
        // ==========================================
        // 1. Prevent duplicate vaccination
        // ==========================================

        if (await AlreadyVaccinatedAsync(
            record.ChildID,
            record.VaccineID,
            record.DoseNumber))
        {
            throw new Exception(
                "This vaccine dose has already been recorded for this child.");
        }

        // ==========================================
        // 2. Verify child exists
        // ==========================================

        var childExists = await _context.Children
            .AnyAsync(c => c.ChildID == record.ChildID);

        if (!childExists)
            throw new Exception("Child not found.");

        // ==========================================
        // 3. Verify vaccine exists
        // ==========================================

        var vaccineExists = await _context.Vaccines
            .AnyAsync(v => v.VaccineID == record.VaccineID);

        if (!vaccineExists)
            throw new Exception("Vaccine not found.");

        // ==========================================
        // 4. Inventory is REQUIRED for clinic
        // ==========================================

        if (!record.InventoryID.HasValue)
        {
            throw new Exception(
                "InventoryID is required when recording a clinic vaccination.");
        }

        // ==========================================
        // 5. Find inventory
        // ==========================================

        var inventory = await _context.VaccineInventory
            .FirstOrDefaultAsync(i =>
                i.InventoryID == record.InventoryID.Value);

        if (inventory == null)
            throw new Exception("Vaccine inventory not found.");

        // ==========================================
        // 6. Make sure inventory belongs to vaccine
        // ==========================================

        if (inventory.VaccineID != record.VaccineID)
        {
            throw new Exception(
                "The selected inventory does not belong to the selected vaccine.");
        }

        // ==========================================
        // 7. Check inventory status
        // ==========================================

        if (!inventory.Status)
        {
            throw new Exception(
                "The selected vaccine inventory is inactive.");
        }

        // ==========================================
        // 8. Check expiration
        // ==========================================

        if (inventory.ExpirationDate.Date < DateTime.Today)
        {
            throw new Exception(
                "The selected vaccine inventory has expired.");
        }

        // ==========================================
        // 9. Check stock
        // ==========================================

        if (inventory.CurrentQuantity <= 0)
        {
            throw new Exception(
                "No vaccine stock remaining.");
        }

        // ==========================================
        // 10. Deduct inventory
        // ==========================================

        inventory.CurrentQuantity--;

        inventory.UpdatedAt = DateTime.UtcNow;

        // ==========================================
        // 11. Create vaccination record
        // ==========================================

        record.VaccinationRecordID = Guid.NewGuid();

        record.RecordCode =
            $"VR-{Guid.NewGuid().ToString("N")[..12].ToUpper()}";

        record.Status = "Completed";

        record.CreatedAt = DateTime.UtcNow;

        // ==========================================
        // 12. Find matching pending timeline
        // ==========================================

        var timeline = await _context.VaccinationTimelines
            .FirstOrDefaultAsync(t =>
                t.ChildID == record.ChildID &&
                t.VaccineID == record.VaccineID &&
                t.DoseNumber == record.DoseNumber &&
                t.Status == "Pending");

        // ==========================================
        // 13. Complete and link timeline
        // ==========================================

        if (timeline != null)
        {
            timeline.Status = "Completed";

            timeline.VaccinationRecordID =
                record.VaccinationRecordID;

            timeline.UpdatedAt = DateTime.UtcNow;

            record.TimelineID =
                timeline.TimelineID;
        }

        // ==========================================
        // 14. Save everything
        // ==========================================

        await _context.VaccinationRecords.AddAsync(record);

        await _context.SaveChangesAsync();

        // ==========================================
        // 15. Commit transaction
        // ==========================================

        await transaction.CommitAsync();
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}

public async Task RecordHistoricalVaccinationsAsync(
    HistoricalVaccinationSubmissionDto submission)
{
    await using var transaction =
        await _context.Database.BeginTransactionAsync();

    try
    {
        // 1. Verify child exists
        var childExists = await _context.Children
            .AnyAsync(c => c.ChildID == submission.ChildID);

        if (!childExists)
            throw new Exception("Child not found.");

        // 2. Nothing to save
        if (submission.Vaccinations == null ||
    !submission.Vaccinations.Any())
        {
            throw new Exception(
                "No historical vaccination records were provided.");
        }

        // 3. Process each historical vaccination
        foreach (var item in submission.Vaccinations)
        {
            // Verify vaccine exists
            var vaccineExists = await _context.Vaccines
                .AnyAsync(v => v.VaccineID == item.VaccineID);

            if (!vaccineExists)
                throw new Exception(
                    $"Vaccine {item.VaccineID} not found.");

            // Prevent duplicate vaccine + dose for this child
            var alreadyExists = await AlreadyVaccinatedAsync(
                submission.ChildID,
                item.VaccineID,
                item.DoseNumber);

            if (alreadyExists)
                throw new Exception(
                    $"Dose {item.DoseNumber} for vaccine {item.VaccineID} "
                    + "has already been recorded.");

            // Create historical record
            var record = new VaccinationRecord
            {
                VaccinationRecordID = Guid.NewGuid(),

                ChildID = submission.ChildID,

                VaccineID = item.VaccineID,
                RecordCode = $"HIST-{Guid.NewGuid().ToString("N")[..20]}".ToUpper(),

                // Historical record:
                // no clinic inventory was used.
                InventoryID = null,

                // Not linked to a timeline yet.
                TimelineID = null,

                DoseNumber = item.DoseNumber,

                VaccinationDate = item.VaccinationDate,

                Status = "Completed",

                CreatedAt = DateTime.UtcNow
            };

            await _context.VaccinationRecords.AddAsync(record);
        }

        // 4. Save all records together
        await _context.SaveChangesAsync();

        // 5. Commit transaction
        await transaction.CommitAsync();
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}
    }
}