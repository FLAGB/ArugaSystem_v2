using Microsoft.AspNetCore.Mvc;
using AndroidWebAPI.Data;
using AndroidWebAPI.Models;

namespace AndroidWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChildrenController : ControllerBase
    {
        private readonly IChildrenRepository _repository;

        public ChildrenController(IChildrenRepository repository)
        {
            _repository = repository;
        }

        // ── CREATE: POST /api/Children ────────────────────────────
        [HttpPost]
        public async Task<IActionResult> CreateChild([FromBody] CreateChildDto dto)
        {
            // Input validation only — no DB or business logic here.
            if (string.IsNullOrWhiteSpace(dto.FirstName))
                return BadRequest(new { message = "FirstName is required." });
            if (string.IsNullOrWhiteSpace(dto.LastName))
                return BadRequest(new { message = "LastName is required." });
            if (dto.BirthDate == default)
                return BadRequest(new { message = "BirthDate is required." });
            if (dto.Parents == null || !dto.Parents.Any())
                return BadRequest(new { message = "At least one parent link is required." });

            try
            {
                var child = await _repository.CreateChildAsync(dto);

                return CreatedAtAction(nameof(GetChildrenByParent), new { parentId = dto.Parents.First().ParentID }, new
                {
                    childID = child.ChildID,
                    firstName = child.FirstName,
                    middleName = child.MiddleName,
                    lastName = child.LastName,
                    birthDate = child.BirthDate,
                    placeOfBirth = child.PlaceOfBirth,
                    sex = child.Sex,
                    barangay = child.Barangay,
                    address = child.Address,
                    healthCenter = child.HealthCenter,
                    allergies = child.Allergies,
                    birthHeight = child.BirthHeight,
                    birthWeight = child.BirthWeight,
                    parents = child.ParentRelationships.Select(r => new
                    {
                        parentID = r.ParentID,
                        relationshipType = r.RelationshipType,
                        isPrimaryContact = r.IsPrimaryContact,
                        canReceiveNotifications = r.CanReceiveNotifications
                    })
                });
            }
            catch (ParentNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred: " + ex.Message });
            }
        }

        // ── UPDATE: PUT /api/Children/{id} ────────────────────────
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateChild(Guid id, [FromBody] UpdateChildDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FirstName))
                return BadRequest(new { message = "FirstName is required." });
            if (string.IsNullOrWhiteSpace(dto.LastName))
                return BadRequest(new { message = "LastName is required." });

            try
            {
                var updated = await _repository.UpdateChildAsync(id, dto);

                if (updated == null)
                    return NotFound(new { message = "Child not found." });

                return Ok(new { message = "Child updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred: " + ex.Message });
            }
        }

        // ── READ: GET /api/Children/parent/{parentId} ─────────────
        [HttpGet("parent/{parentId}")]
        public async Task<IActionResult> GetChildrenByParent(Guid parentId)
        {
            try
            {
                var children = await _repository.GetByParentAsync(parentId);

                var result = children.Select(c =>
                {
                    var relationship = c.ParentRelationships
                        .FirstOrDefault(r => r.ParentID == parentId);

                    return new
                    {
                        childID = c.ChildID,
                        firstName = c.FirstName,
                        middleName = c.MiddleName,
                        lastName = c.LastName,
                        birthDate = c.BirthDate,
                        placeOfBirth = c.PlaceOfBirth,
                        allergies = c.Allergies,
                        sex = c.Sex,
                        healthCenter = c.HealthCenter,
                        barangay = c.Barangay,
                        address = c.Address,
                        birthHeight = c.BirthHeight,
                        birthWeight = c.BirthWeight,

                        // Computed: which role does the logged-in parent have for this child
                        myRelationship = relationship?.RelationshipType,
                        isPrimaryContact = relationship?.IsPrimaryContact ?? false,
                        canReceiveNotifications = relationship?.CanReceiveNotifications ?? false
                    };
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred: " + ex.Message });
            }
        }

        // ── READ: GET /api/Children/all ────────────────────────────
        [HttpGet("all")]
public async Task<IActionResult> GetAllChildren()
{
    try
    {
        var children = await _repository.GetAllAsync();

        var result = children.Select(c => new
        {
            childID = c.ChildID,
            firstName = c.FirstName,
            middleName = c.MiddleName,
            lastName = c.LastName,
            birthDate = c.BirthDate,
            placeOfBirth = c.PlaceOfBirth,
            address = c.Address,
            healthCenter = c.HealthCenter,
            barangay = c.Barangay,
            sex = c.Sex,
            allergies = c.Allergies,
            birthHeight = c.BirthHeight,
            birthWeight = c.BirthWeight,
            parentName = GetPrimaryParentName(c),

            parents = c.ParentRelationships.Select(r => new
            {
                parentID = r.ParentID,
                parentName = r.Parent != null ? $"{r.Parent.FirstName} {r.Parent.LastName}".Trim() : null,
                relationshipType = r.RelationshipType,
                isPrimaryContact = r.IsPrimaryContact,
                canReceiveNotifications = r.CanReceiveNotifications
            })
        });

        return Ok(result);
    }
    catch (Exception ex)
    {
        return StatusCode(500, new
        {
            message = "An error occurred while retrieving children.",
            error = ex.Message
        });
    }
}

// ── READ: GET /api/Children/{id} ─────────────────────────────
[HttpGet("{id}")]
public async Task<IActionResult> GetChildById(Guid id)
{
    try
    {
        var children = await _repository.GetAllAsync();

        var child = children.FirstOrDefault(c => c.ChildID == id);

        if (child == null)
        {
            return NotFound(new
            {
                message = "Child not found."
            });
        }

        var result = new
        {
            childID = child.ChildID,
            firstName = child.FirstName,
            middleName = child.MiddleName,
            lastName = child.LastName,
            birthDate = child.BirthDate,
            placeOfBirth = child.PlaceOfBirth,
            address = child.Address,
            healthCenter = child.HealthCenter,
            allergies = child.Allergies,
            barangay = child.Barangay,
            sex = child.Sex,
            birthHeight = child.BirthHeight,
            birthWeight = child.BirthWeight,
            parentName = GetPrimaryParentName(child),

            parents = child.ParentRelationships.Select(r => new
            {
                parentID = r.ParentID,
                parentName = r.Parent != null ? $"{r.Parent.FirstName} {r.Parent.LastName}".Trim() : null,
                relationshipType = r.RelationshipType,
                isPrimaryContact = r.IsPrimaryContact,
                canReceiveNotifications = r.CanReceiveNotifications
            })
        };

        return Ok(result);
    }
    catch (Exception ex)
    {
        return StatusCode(500, new
        {
            message = "An error occurred while retrieving the child.",
            error = ex.Message
        });
    }
}

// Primary contact's display name — falls back to the first linked parent
// if none is flagged primary. Mirrors the same lookup used for vaccination
// notifications and the Vaccination Records list.
private static string? GetPrimaryParentName(Child child)
{
    var relationship = child.ParentRelationships?.FirstOrDefault(r => r.IsPrimaryContact && r.Status == "Active")
        ?? child.ParentRelationships?.FirstOrDefault(r => r.Status == "Active");

    return relationship?.Parent != null
        ? $"{relationship.Parent.FirstName} {relationship.Parent.LastName}".Trim()
        : null;
}
    }
}