using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TatumBackendApi.Common.Constants
{
    public class SmsSettings
    {
        public string Provider { get; set; } = "Termii";
        public string ApiKey { get; set; } = null!;
        public string SenderId { get; set; } = "TatumConnect";
        public string BaseUrl { get; set; } = "https://api.ng.termii.com";
    }
}