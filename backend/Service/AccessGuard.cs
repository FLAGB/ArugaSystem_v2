using System.Security.Claims;
using AndroidWebAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace AndroidWebAPI.Services
{
    // Role names in the sign-in token (see AuthController.Login)
    public static class Roles
    {
        public const string Parent = "Parent";
        public const string Healthcare = "Healthcare";   // Doctor or Nurse
        public const string Staff = "Staff";             // Admission Staff
        public const string Admin = "SystemAdmin";

        public const string StaffOrAdmin = Staff + "," + Admin;
        public const string ClinicTeam = Healthcare + "," + Staff + "," + Admin;
    }

    // Keeps each family's records private: a parent can only open their own
    // profile, children, notifications and queue ticket. Clinic personnel
    // (health workers, staff, admin) can see every patient.
    public static class AccessGuard
    {
        public static bool IsParent(ClaimsPrincipal user) => user.IsInRole(Roles.Parent);

        // Users.UserID for personnel, Parents.ParentID for parents
        public static Guid? CallerId(ClaimsPrincipal user) =>
            Guid.TryParse(user.FindFirst("ReferenceID")?.Value, out var id) ? id : null;

        public static bool CanSeeParent(ClaimsPrincipal user, Guid parentId) =>
            !IsParent(user) || CallerId(user) == parentId;

        public static async Task<bool> CanSeeChildAsync(ClaimsPrincipal user, AppDbContext context, Guid childId)
        {
            if (!IsParent(user)) return true;
            var me = CallerId(user);
            return me != null && await context.ChildParentRelationships
                .AnyAsync(r => r.ChildID == childId && r.ParentID == me && r.Status == "Active");
        }
    }
}
