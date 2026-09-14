using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TatumBackendApi.DTOs
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string TokenType { get; set; } = "Bearer";
        public int ExpiresInSeconds { get; set; }
        public string? RefreshToken { get; set; }
        
        public UserDto User { get; set; } = null!;
    }

    
}