using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AndroidWebAPI.Models
{
    [Table("QueueQRCodes")]
    public class QueueQRCode
    {
        [Key]
        public Guid QRCodeID { get; set; }

        public string Token { get; set; } = string.Empty;

        public DateTime QRDate { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime ValidUntil { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }
    }
}