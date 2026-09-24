using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AndroidWebAPI.Data;
using AndroidWebAPI.Models;
using AndroidWebAPI.DTOs;

namespace AndroidWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VaccinationRecordsController : ControllerBase
    {
        private readonly IVaccinationRecordRepository _repository;
        private readonly AppDbContext _context;
        private readonly ILogger<VaccinationRecordsController> _logger;

        public VaccinationRecordsController(
            IVaccinationRecordRepository repository,
            AppDbContext context,
            ILogger<VaccinationRecordsController> logger)
        {
            _repository = repository;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _repository.GetAllAsync());
        }

        // Explicit "/all" route — the frontend (Doctor Vaccination Records,
        // Doctor Calendar, Doctor Reports) calls GET /api/VaccinationRecords/all.
        // Without this, "all" fell through to GetById(Guid id) below and
        // failed model binding ("The value 'all' is not valid.", 400).
        [HttpGet("all")]
        public async Task<IActionResult> GetAllExplicit()
        {
            var records = await _repository.GetAllAsync();
            return Ok(ShapeRecords(records));
        }

        // GetAllAsync() returns raw VaccinationRecord entities, which serialize
        // with the bare PascalCase model properties (ChildID, VaccineID, etc.)
        // instead of the childName/vaccineName/administeredByName shape the
        // frontend expects (same shape GetByChildAsync's DTO already produces).
        // This projects those entities into that shape without touching
        // GetAllAsync() itself, since Stats/PendingToday/CompletedToday below
        // rely on it returning navigable entities, not a DTO.
        private static IEnumerable<object> ShapeRecords(IEnumerable<VaccinationRecord> records)
        {
            return records.Select(r => new
            {
                vaccinationRecordID = r.VaccinationRecordID,
                recordCode = r.RecordCode,
                childID = r.ChildID,
                childName = r.Child != null ? $"{r.Child.FirstName} {r.Child.LastName}".Trim() : "Unknown",
                parentName = GetPrimaryParentName(r.Child),
                vaccineID = r.VaccineID,
                vaccineName = r.Vaccine != null ? r.Vaccine.VaccineName : $"Vaccine {r.VaccineID}",
                doseNumber = r.DoseNumber,
                vaccinationDate = r.VaccinationDate,
                status = r.Status,
                administeredByUserID = r.AdministeredByUserID,
                administeredByName = r.AdministeredBy != null
                    ? $"{r.AdministeredBy.FirstName} {r.AdministeredBy.LastName}".Trim()
                    : null,
                nurseObservation = r.NurseObservation,
                doctorDiagnosis = r.DoctorDiagnosis,
                doctorDiagnosedByUserID = r.DoctorDiagnosedByUserID,
                doctorDiagnosedAt = r.DoctorDiagnosedAt,
                lotNumber = r.Inventory != null ? r.Inventory.LotNumber : null,
            });
        }

        // Mirrors the primary-contact lookup in NotifyParentAsync below.
        private static string? GetPrimaryParentName(Child? child)
        {
            var primaryParent = child?.ParentRelationships?
                .FirstOrDefault(pr => pr.IsPrimaryContact && pr.Status == "Active")?
                .Parent;

            return primaryParent != null
                ? $"{primaryParent.FirstName} {primaryParent.LastName}".Trim()
                : null;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var record = await _repository.GetByIdAsync(id);
            if (record == null) return NotFound();
            return Ok(record);
        }

        [HttpGet("child/{childId}")]
        public async Task<IActionResult> GetByChild(Guid childId)
        {
            return Ok(await _repository.GetByChildAsync(childId));
        }

        [HttpPost]
        public async Task<IActionResult> RecordVaccination([FromBody] RecordVaccinationDto dto)
        {
            var record = new VaccinationRecord
            {
                ChildID = dto.ChildID,
                VaccineID = dto.VaccineID,
                DoseNumber = dto.DoseNumber,
                VaccinationDate = dto.VaccinationDate,
                InventoryID = dto.InventoryID,
                AdministeredByUserID = dto.AdministeredByUserID,
                NurseObservation = dto.NurseObservation,
                DoctorDiagnosis = dto.DoctorDiagnosis,
                DoctorDiagnosedByUserID = dto.DoctorDiagnosedByUserID,
                DoctorDiagnosedAt = dto.DoctorDiagnosedAt
            };

            try
            {
                await _repository.RecordVaccinationAsync(record);
                await NotifyParentAsync(record);

                return Ok(new
                {
                    message = "Vaccination recorded successfully.",
                    vaccinationRecordID = record.VaccinationRecordID,
                    recordCode = record.RecordCode
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "RecordVaccination failed for Child {ChildId}", dto.ChildID);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/complete")]
        public async Task<IActionResult> CompleteVaccination(Guid id, [FromBody] CompleteVaccinationDto dto)
        {
            try
            {
                var record = await _repository.CompleteVaccinationAsync(id, dto);
                await NotifyParentAsync(record);

                return Ok(new
                {
                    message = "Vaccination recorded successfully.",
                    vaccinationRecordID = record.VaccinationRecordID
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "CompleteVaccination failed for Record {RecordId}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        // Updates the doctor's diagnosis/clinical notes on an EXISTING
        // (already-completed) vaccination record. Separate from
        // RecordVaccination/CompleteVaccination, which only accept a
        // diagnosis at the moment a dose is recorded — this lets a doctor
        // go back and add or correct it afterward from the patient record
        // view. Does not touch inventory, timeline, or Status, and does
        // not send a parent notification (diagnosis edits aren't a "dose
        // administered" event).
        [HttpPatch("{id}/diagnosis")]
        public async Task<IActionResult> UpdateDiagnosis(Guid id, [FromBody] UpdateDiagnosisDto dto)
        {
            try
            {
                var record = await _repository.UpdateDiagnosisAsync(id, dto);

                return Ok(new
                {
                    message = "Diagnosis updated successfully.",
                    vaccinationRecordID = record.VaccinationRecordID,
                    doctorDiagnosis = record.DoctorDiagnosis,
                    doctorDiagnosedByUserID = record.DoctorDiagnosedByUserID,
                    doctorDiagnosedAt = record.DoctorDiagnosedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "UpdateDiagnosis failed for Record {RecordId}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        // Notifies the child's primary parent that a dose was completed.
        // Logs (instead of silently returning) when it can't, so this never
        // fails invisibly again.
        private async Task NotifyParentAsync(VaccinationRecord record)
        {
            var child = await _context.Children
                .Include(c => c.ParentRelationships)
                .FirstOrDefaultAsync(c => c.ChildID == record.ChildID);

            if (child == null)
            {
                _logger.LogWarning("Vaccination {RecordId} completed but Child {ChildId} not found — no notification sent.",
                    record.VaccinationRecordID, record.ChildID);
                return;
            }

            var primaryParent = child.ParentRelationships?
                .FirstOrDefault(r => r.IsPrimaryContact && r.Status == "Active");

            if (primaryParent == null)
            {
                _logger.LogWarning("Vaccination {RecordId} completed for Child {ChildId} but no active primary parent link found — no notification sent.",
                    record.VaccinationRecordID, record.ChildID);
                return;
            }

            var vaccine = await _context.Vaccines.FindAsync(record.VaccineID);
            string childName = $"{child.FirstName} {child.LastName}";
            string vaccineName = vaccine?.VaccineName ?? $"Vaccine {record.VaccineID}";
            string dateLabel = record.VaccinationDate.ToString("MMMM d, yyyy");

            _context.Notifications.Add(new Notification
            {
                ParentID = primaryParent.ParentID,
                ChildID = record.ChildID,
                VaccineID = record.VaccineID,
                DoseNumber = record.DoseNumber,
                Type = "Completed",
                Title = $"Vaccine administered — {childName}",
                Message = $"{vaccineName} (Dose {record.DoseNumber}) was administered to {childName} on {dateLabel}.",
                ScheduledDate = record.VaccinationDate,
                IsRead = false,
                CreatedAt = DateTime.Now
            });

            await _context.SaveChangesAsync();
        }

        [HttpPut]
        public async Task<IActionResult> Update(VaccinationRecord record)
        {
            await _repository.UpdateAsync(record);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("historical")]
        public async Task<IActionResult> RecordHistoricalVaccinations([FromBody] HistoricalVaccinationSubmissionDto submission)
        {
            await _repository.RecordHistoricalVaccinationsAsync(submission);
            return Ok(new { message = "Historical vaccination records saved successfully." });
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            var allRecords = await _repository.GetAllAsync();

            return Ok(new
            {
                vaccinatedToday = allRecords.Count(r => r.Status == "Completed" && r.VaccinationDate >= today && r.VaccinationDate < tomorrow),
                pendingToday = allRecords.Count(r => r.Status == "Scheduled" && r.VaccinationDate >= today && r.VaccinationDate < tomorrow),
                weeklyTotal = allRecords.Count(r => r.Status == "Completed" && r.VaccinationDate >= weekStart && r.VaccinationDate < tomorrow),
                missedTotal = allRecords.Count(r => r.Status == "Deferred" || r.Status == "Cancelled")
            });
        }

        [HttpGet("pending-today")]
        public async Task<IActionResult> GetPendingToday()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var records = (await _repository.GetAllAsync())
                .Where(r => r.Status == "Scheduled" && r.VaccinationDate >= today && r.VaccinationDate < tomorrow)
                .Select(r => new
                {
                    recordId = r.VaccinationRecordID,
                    childId = r.ChildID,
                    childName = r.Child != null ? $"{r.Child.FirstName} {r.Child.LastName}".Trim() : "Unknown",
                    vaccineId = r.VaccineID,
                    vaccineName = r.Vaccine != null ? r.Vaccine.VaccineName : "Unknown",
                    doseNumber = r.DoseNumber,
                    vaccinationDate = r.VaccinationDate,
                    status = r.Status
                })
                .ToList();

            return Ok(records);
        }

        [HttpGet("completed-today")]
        public async Task<IActionResult> GetCompletedToday()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var records = (await _repository.GetAllAsync())
                .Where(r => r.Status == "Completed" && r.VaccinationDate >= today && r.VaccinationDate < tomorrow)
                .OrderByDescending(r => r.VaccinationDate)
                .Select(r => new
                {
                    recordId = r.VaccinationRecordID,
                    childId = r.ChildID,
                    childName = r.Child != null ? $"{r.Child.FirstName} {r.Child.LastName}".Trim() : "Unknown",
                    vaccineName = r.Vaccine != null ? r.Vaccine.VaccineName : "Unknown",
                    doseNumber = r.DoseNumber,
                    vaccinationDate = r.VaccinationDate
                })
                .ToList();

            return Ok(records);
        }
    }
}