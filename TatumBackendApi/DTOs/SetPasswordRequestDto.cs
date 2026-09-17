using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TatumBackendApi.DTOs
{
    public class SetPasswordRequestDto
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required, MinLength(8), MaxLength(100),
        RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%?&])[A-Za-z\d@$!%?&]{8,100}$")]
        public string Password { get; set; } = string.Empty;

        [Required, Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}