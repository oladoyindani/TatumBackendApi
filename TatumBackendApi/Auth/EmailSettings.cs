using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TatumBackendApi.Auth
{
    public class EmailSettings
    {
        // Email provider: "Smtp" or "SendGrid"
        public string Provider { get; set; } = "Smtp";

        // Email address that appears as the sender
        public string FromEmail { get; set; } = null!;

        // Sender display name
        public string FromName { get; set; } = "TatumConnect";

        // Frontend URL used for password password setup
        // Example: https://app.tatumconnect.com
        public string FrontendBaseUrl { get; set; } = null!;

        public SmtpSettings Smtp { get; set; } = new();

        public SendGridSettings SendGrid { get; set; } = new();
    }

    public class SmtpSettings
    {
        public string Host { get; set; } = null!;
        public int Port { get; set; } = 587;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool EnableSsl { get; set; } = true;
    }

    public class SendGridSettings
    {
        public string ApiKey { get; set; } = null!;

        public string ApiUrl { get; set; } = "https://api.sendgrid.com/v3/mail/send";
    }
}