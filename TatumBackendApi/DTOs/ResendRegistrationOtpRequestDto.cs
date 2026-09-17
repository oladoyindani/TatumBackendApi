using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TatumBackendApi.DTOs
{
    public class ResendRegistrationOtpRequestDto
    {
        public string Email { get; set; } = string.Empty;
    }
}