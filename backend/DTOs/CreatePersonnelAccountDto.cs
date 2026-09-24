namespace AndroidWebAPI.DTOs
{
    public class CreatePersonnelAccountDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;

        public string? ContactNo { get; set; }

        public string Role { get; set; } = string.Empty;

        public string? LicenseNumber { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }
    }
}