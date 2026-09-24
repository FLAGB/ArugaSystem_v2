namespace AndroidWebAPI.Models
{
    public class Notification
    {
        public Guid NotificationID { get; set; }

        // Parent-facing notifications (vaccine reminders, stock alerts a parent
        // should see) keep using ParentID/ChildID as before.
        public Guid? ParentID { get; set; }
        public Guid? ChildID { get; set; }

        // Staff-facing notifications (doctor/nurse bell) use UserID instead.
        // Exactly one of ParentID or UserID should be set per row.
        public Guid? UserID { get; set; }
        public int? VaccineID { get; set; }
        public int? DoseNumber { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime? ScheduledDate { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}