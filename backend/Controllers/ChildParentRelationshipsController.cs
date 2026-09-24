using Microsoft.AspNetCore.Mvc;
using AndroidWebAPI.Data;
using AndroidWebAPI.DTOs;

namespace AndroidWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChildParentRelationshipsController : ControllerBase
    {
        private readonly IChildParentRelationshipRepository _repository;

        public ChildParentRelationshipsController(
            IChildParentRelationshipRepository repository)
        {
            _repository = repository;
        }

        // POST /api/ChildParentRelationships
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateChildParentRelationshipDto dto)
        {
            if (dto.ChildID == Guid.Empty)
                return BadRequest(new
                {
                    message = "ChildID is required."
                });

            if (dto.ParentID == Guid.Empty)
                return BadRequest(new
                {
                    message = "ParentID is required."
                });

            if (string.IsNullOrWhiteSpace(dto.RelationshipType))
                return BadRequest(new
                {
                    message = "RelationshipType is required."
                });

            try
            {
                var relationship =
                    await _repository.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetByChild),
                    new { childID = dto.ChildID },
                    new
                    {
                        relationshipID =
                            relationship!.RelationshipID,

                        childID =
                            relationship.ChildID,

                        parentID =
                            relationship.ParentID,

                        relationshipType =
                            relationship.RelationshipType,

                        isPrimaryContact =
                            relationship.IsPrimaryContact,

                        canReceiveNotifications =
                            relationship.CanReceiveNotifications,

                        status =
                            relationship.Status,

                        createdAt =
                            relationship.CreatedAt
                    });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // GET /api/ChildParentRelationships/child/{childID}
        [HttpGet("child/{childID}")]
        public async Task<IActionResult> GetByChild(Guid childID)
        {
            try
            {
                var relationships =
                    await _repository.GetByChildAsync(childID);

                var result = relationships.Select(r => new
                {
                    relationshipID = r.RelationshipID,
                    childID = r.ChildID,
                    parentID = r.ParentID,
                    relationshipType = r.RelationshipType,
                    isPrimaryContact = r.IsPrimaryContact,
                    canReceiveNotifications =
                        r.CanReceiveNotifications,
                    status = r.Status,
                    createdAt = r.CreatedAt
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ex.Message
                });
            }
        }

        // GET /api/ChildParentRelationships/parent/{parentID}
        [HttpGet("parent/{parentID}")]
        public async Task<IActionResult> GetByParent(Guid parentID)
        {
            try
            {
                var relationships =
                    await _repository.GetByParentAsync(parentID);

                var result = relationships.Select(r => new
                {
                    relationshipID = r.RelationshipID,
                    childID = r.ChildID,
                    parentID = r.ParentID,
                    relationshipType = r.RelationshipType,
                    isPrimaryContact = r.IsPrimaryContact,
                    canReceiveNotifications =
                        r.CanReceiveNotifications,
                    status = r.Status,
                    createdAt = r.CreatedAt
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ex.Message
                });
            }
        }

        // DELETE /api/ChildParentRelationships/{relationshipID}
        [HttpDelete("{relationshipID}")]
        public async Task<IActionResult> Delete(
            Guid relationshipID)
        {
            try
            {
                var deleted =
                    await _repository.DeleteAsync(relationshipID);

                if (!deleted)
                    return NotFound(new
                    {
                        message = "Relationship not found."
                    });

                return Ok(new
                {
                    message =
                        "Parent-child relationship removed successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ex.Message
                });
            }
        }
    }
}