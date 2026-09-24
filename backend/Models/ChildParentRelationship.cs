using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AndroidWebAPI.Models
{
    public class ChildParentRelationship
    {
        [Key]
        public Guid RelationshipID { get; set; }

        [ForeignKey(nameof(Child))]
        public Guid ChildID { get; set; }

        [ForeignKey(nameof(Parent))]
        public Guid ParentID { get; set; }

        public string RelationshipType { get; set; } = string.Empty;

        public bool IsPrimaryContact { get; set; } = false;

        public bool CanReceiveNotifications { get; set; } = true;

        public string Status { get; set; } = "Active";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
      public virtual Child Child { get; set; } = null!;
        public virtual Parent Parent { get; set; } = null!;
    }
}