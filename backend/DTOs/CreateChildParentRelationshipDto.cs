namespace AndroidWebAPI.DTOs
{
    public class CreateChildParentRelationshipDto
    {
        public Guid ChildID { get; set; }

        public Guid ParentID { get; set; }

        public string RelationshipType { get; set; } = string.Empty;

        public bool IsPrimaryContact { get; set; } = false;

        public bool CanReceiveNotifications { get; set; } = true;
    }
}