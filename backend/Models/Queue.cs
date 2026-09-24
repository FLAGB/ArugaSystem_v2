using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AndroidWebAPI.Models
{
    [Table("Queues")]
    public class Queue
    {
        [Key]
        public Guid QueueID { get; set; }

        public Guid ParentID { get; set; }

        public int QueueNumber { get; set; }

        public DateTime QueueDate { get; set; }

        public string Status { get; set; } = "Waiting";

        // Which clinic room the patient was called into (set by call-next).
        // Added for the Doctor dashboard board; nullable so existing rows
        // and code paths that don't use rooms are unaffected.
        public int? AssignedRoomID { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Navigation Property
        [ForeignKey(nameof(ParentID))]
        public virtual Parent? Parent { get; set; }

        public virtual ICollection<QueueChild> QueueChildren { get; set; }
            = new List<QueueChild>();
    }
}