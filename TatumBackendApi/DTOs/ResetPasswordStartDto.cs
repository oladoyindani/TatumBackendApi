using System.ComponentModel.DataAnnotations;

namespace TatumBackendApi.Dtos
{
    public class ResetPasswordStartDto
    {


        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [MaxLength(254, ErrorMessage = "Email address is too long.")]

        public string Email { get; set; } = string.Empty;
    }
}
