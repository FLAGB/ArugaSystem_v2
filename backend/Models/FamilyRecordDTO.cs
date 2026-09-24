namespace AndroidWebAPI.Models
{
    public class FamilyRecordDto
    {
        // Parent
        public Guid ParentID { get; set; }
        public string ParentFullName { get; set; } = string.Empty;
        public string ContactNo { get; set; } = string.Empty;

        // Child
        public Guid ChildID { get; set; }
        public string ChildFullName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Sex { get; set; } = string.Empty;
        public int? Barangay { get; set; }

        // Relationship
        public Guid RelationshipID { get; set; }
        public string RelationshipType { get; set; } = string.Empty;
        public bool IsPrimaryContact { get; set; }
        public bool CanReceiveNotifications { get; set; }
    }
}