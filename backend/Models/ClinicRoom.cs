using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AndroidWebAPI.Models
{
    [Table("ClinicRooms")]
    public class ClinicRoom
    {
        [Key]
        public int RoomID { get; set; }

        public string RoomNumber { get; set; } = string.Empty;

        public Guid? AssignedDoctorID { get; set; }

        public bool IsOccupied { get; set; }

        public Guid? CurrentChildID { get; set; }
    }
}