using System.ComponentModel.DataAnnotations;

namespace TatumBackendApi.DTOs
{
    public class VerifyRegistrationOtpRequestDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please provide a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "OTP is required.")]
        public string Otp { get; set; } = string.Empty;
    }
}
