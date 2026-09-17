using System.ComponentModel.DataAnnotations;

namespace TatumBackendApi.Dtos
{
    public class ResetPasswordDto
    {

        public string Otp { get; set; } = string.Empty;


        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [MaxLength(254, ErrorMessage = "Email address is too long.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [MaxLength(100, ErrorMessage = "Password cannot exceed 100 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,100}$",
         ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string NewPassword { get; set; } = string.Empty;

    }
}
