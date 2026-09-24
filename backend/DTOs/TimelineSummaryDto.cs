namespace AndroidWebAPI.DTOs
{
    public class TimelineSummaryDto
    {
        public Guid ChildID { get; set; }

        public bool HasTimeline { get; set; }

        public int Pending { get; set; }

        public int Completed { get; set; }

        public int Missed { get; set; }
    }
}