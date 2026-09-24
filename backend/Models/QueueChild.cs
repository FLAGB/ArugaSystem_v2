using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AndroidWebAPI.Models
{
    [Table("QueueChildren")]
    public class QueueChild
    {
        [Key]
        public Guid QueueChildID { get; set; }

        public Guid QueueID { get; set; }

        public Guid ChildID { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(QueueID))]
        public virtual Queue? Queue { get; set; }

        [ForeignKey(nameof(ChildID))]
        public virtual Child? Child { get; set; }
    }
}