using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AndroidWebAPI.Models
{
    [Table("QueueQRSettings")]
    public class QueueQRSetting
    {
        [Key]
        public int SettingID { get; set; }

        public bool IsEnabled { get; set; } = false;

        public DateTime UpdatedAt { get; set; }
    }
}