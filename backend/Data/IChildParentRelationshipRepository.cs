using AndroidWebAPI.Models;
using AndroidWebAPI.DTOs;

namespace AndroidWebAPI.Data
{
    public interface IChildParentRelationshipRepository
    {
        Task<ChildParentRelationship?> CreateAsync(
            CreateChildParentRelationshipDto dto);

        Task<IEnumerable<ChildParentRelationship>> GetByChildAsync(
            Guid childID);

        Task<IEnumerable<ChildParentRelationship>> GetByParentAsync(
            Guid parentID);

        Task<bool> DeleteAsync(Guid relationshipID);
    }
}