using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AndroidWebAPI.Models
{
    [Table("Accounts")]
    public class Account
    {
        [Key]
        public Guid AccountID { get; set; }

        public string Username { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string AccountType { get; set; } = string.Empty;

        public Guid ReferenceID { get; set; }

        public bool Status { get; set; } = true;

        public bool MustChangePassword { get; set; } = false;

        public int FailedLoginAttempts { get; set; } = 0;

        public DateTime? LockedUntil { get; set; }

        public DateTime? LastLogin { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }
    }
}