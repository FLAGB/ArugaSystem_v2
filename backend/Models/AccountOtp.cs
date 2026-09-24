using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AndroidWebAPI.Models
{
    [Table("AccountOtps")]
    public class AccountOtp
    {
        [Key]
        public Guid OTPID { get; set; }

        public Guid? ParentID { get; set; }

        public Guid? UserID { get; set; }

        [Required]
        [MaxLength(255)]
        public string OTPCodeHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Purpose { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public bool IsUsed { get; set; } = false;

        public int AttemptCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; }

        public DateTime? UsedAt { get; set; }

        // Navigation properties
        [ForeignKey(nameof(ParentID))]
        public virtual Parent? Parent { get; set; }

        [ForeignKey(nameof(UserID))]
        public virtual User? User { get; set; }
    }
}