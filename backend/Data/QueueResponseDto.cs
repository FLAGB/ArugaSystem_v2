namespace AndroidWebAPI.DTOs
{
    public class QueueResponseDto
    {
        public Guid QueueID { get; set; }
        public int QueueNumber { get; set; }
        public string? BarangayNo { get; set; }
        public string RequestBy { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime QueueDate { get; set; }

        public List<QueueChildResponseDto> Children { get; set; }
            = new List<QueueChildResponseDto>();
    }

    public class QueueChildResponseDto
    {
        public Guid ChildID { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}