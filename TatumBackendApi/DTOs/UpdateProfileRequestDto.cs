namespace TatumBackendApi.DTOs
{
    public class UpdateProfileRequestDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public string? Department { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}
