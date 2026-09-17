using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TatumBackendApi.DTOs
{
    public class AdminInviteRequestDto
    {
        [Required, EmailAddress, MaxLength(254)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(100), RegularExpression(@"^[a-zA-ZÀ-ÿ\s'-]+$")]
        public string? FirstName { get; set; }
        // ...LastName same pattern
        public string? LastName { get; set; }

        [Required, RegularExpression(@"^(?:\+234|0)[789][01]\d{8}$")]
        public string Phone { get; set; } = string.Empty;

        public string? Department { get; set; }

        [Required, MaxLength(50)]
        public string Role { get; set; } = string.Empty;
    }
}