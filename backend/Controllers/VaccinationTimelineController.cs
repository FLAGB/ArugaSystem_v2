using AndroidWebAPI.Data;
    using Microsoft.AspNetCore.Mvc;
    using AndroidWebAPI.DTOs;

    namespace AndroidWebAPI.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class VaccinationTimelineController : ControllerBase
        {
            private readonly IVaccinationTimelineRepository _repository;

            public VaccinationTimelineController(IVaccinationTimelineRepository repository)
            {
                _repository = repository;
            }

            // GET: api/VaccinationTimeline
            [HttpGet]
            public async Task<IActionResult> GetAll()
            {
                var timelines = await _repository.GetAllAsync();
                return Ok(timelines);
            }

            // GET: api/VaccinationTimeline/{timelineId}
            [HttpGet("{timelineId}")]
            public async Task<IActionResult> GetById(Guid timelineId)
            {
                var timeline = await _repository.GetByIdAsync(timelineId);

                if (timeline == null)
                    return NotFound();

                return Ok(timeline);
            }

            // GET: api/VaccinationTimeline/child/{childId}
            [HttpGet("child/{childId}")]
public async Task<IActionResult> GetByChild(Guid childId)
{
    var timelines = await _repository.GetByChildAsync(childId);

    var result = timelines.Select(t => new
    {
        timelineID = t.TimelineID,
        vaccineID = t.VaccineID,
        vaccineName = t.Vaccine?.VaccineName ?? "Unknown",
        doseNumber = t.DoseNumber,
        expectedDate = t.ExpectedDate,
        scheduledDate = t.ScheduledDate,
        status = t.Status,
        vaccinationRecordID = t.VaccinationRecordID
    });

    return Ok(result);
}

            // POST: api/VaccinationTimeline/generate/{childId}
            [HttpPost("generate/{childId}")]
            public async Task<IActionResult> Generate(Guid childId)
            {
                await _repository.GenerateTimelineAsync(childId);

                return Ok(new
                {
                    message = "Vaccination timeline generated successfully."
                });
            }

            // PUT: api/VaccinationTimeline/complete/{timelineId}
            [HttpPut("complete/{timelineId}")]
            public async Task<IActionResult> MarkCompleted(Guid timelineId)
            {
                await _repository.MarkCompletedAsync(timelineId);

                return Ok(new
                {
                    message = "Vaccination marked as completed."
                });
            }

            // POST: api/VaccinationTimeline/regenerate/{childId}
            [HttpPost("regenerate/{childId}")]
            public async Task<IActionResult> Regenerate(Guid childId)
            {
                await _repository.RegenerateTimelineAsync(childId);

                return Ok(new
                {
                    message = "Vaccination timeline regenerated."
                });
            }

            // GET: api/VaccinationTimeline/due-today
            // Shape expected by the Doctor Home dashboard's "Pending
            // Vaccinations Today" panel — timelineId identifies the
            // VaccinationTimeline row (there is no VaccinationRecords row
            // yet for a Pending dose, so there is deliberately no recordId
            // here). This replaces GET /api/VaccinationRecords/pending-today,
            // which read from VaccinationRecords — a table nothing in the
            // app ever populates with Status "Scheduled" — and so always
            // returned []. VaccinationTimeline (Status "Pending") is the
            // actual source of truth for "what's due."
            [HttpGet("due-today")]
            public async Task<IActionResult> GetDueToday()
            {
                var timelines = await _repository.GetDueTodayAsync();

                var result = timelines.Select(t => new
                {
                    timelineId = t.TimelineID,
                    childId = t.ChildID,
                    childName = t.Child != null ? $"{t.Child.FirstName} {t.Child.LastName}".Trim() : "Unknown",
                    parentName = GetPrimaryParentName(t.Child),
                    vaccineId = t.VaccineID,
                    vaccineName = t.Vaccine != null ? t.Vaccine.VaccineName : $"Vaccine {t.VaccineID}",
                    doseNumber = t.DoseNumber,
                    scheduledDate = t.ScheduledDate,
                });

                return Ok(result);
            }

            // Mirrors VaccinationRecordsController's helper of the same name.
            private static string? GetPrimaryParentName(AndroidWebAPI.Models.Child? child)
            {
                var primaryParent = child?.ParentRelationships?
                    .FirstOrDefault(pr => pr.IsPrimaryContact && pr.Status == "Active")?
                    .Parent;

                return primaryParent != null
                    ? $"{primaryParent.FirstName} {primaryParent.LastName}".Trim()
                    : null;
            }

            // GET: api/VaccinationTimeline/upcoming/{days}
            [HttpGet("upcoming/{days}")]
            public async Task<IActionResult> GetUpcoming(int days)
            {
                var timelines = await _repository.GetUpcomingAsync(days);
                return Ok(timelines);
            }
    [HttpGet("summary/{childId}")]
    public async Task<IActionResult> GetSummary(Guid childId)
    {
        var summary = await _repository.GetTimelineSummaryAsync(childId);
        return Ok(summary);
    }

    [HttpPut("update-missed")]
    public async Task<IActionResult> UpdateMissed()
    {
        await _repository.UpdateMissedVaccinationsAsync();

        return Ok(new
        {
            message = "Missed vaccinations updated successfully."
        });
    }
        }
    }