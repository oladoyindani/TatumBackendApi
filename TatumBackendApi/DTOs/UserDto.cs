using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TatumBackendApi.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }

        public string? Email { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Phone { get; set; }

        public string? Department { get; set; }

        public string? ProfileImageUrl { get; set; }

        public string? Staffid { get; set; }

        public string? Role { get; set; } = null;

        public string? RegistrationOtp { get; set; }
        public bool IsActive {get; set;}

        public DateTime? CreatedAt {get; set;}

        public DateTime? LastLoginAt {get; set;}

        // Customer Accounts
        public List<AccountDto> Accounts { get; set;} = new();
    }
}