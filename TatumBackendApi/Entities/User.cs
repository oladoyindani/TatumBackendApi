using System.ComponentModel.DataAnnotations;
using TatumBackendApi.Common.Constants;

namespace TatumBackendApi.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = null!;
        public string? PasswordHash { get; set; }

        [MaxLength(100)]
        public string? FirstName { get; set; }

        [MaxLength(100)]
        public string? LastName { get; set; }

        [MaxLength(30)]
        public string? Phone { get; set; }

        [MaxLength(150)]
        public string? Department { get; set; }

        [MaxLength(500)]
        public string? ProfileImageUrl { get; set; }

        [MaxLength(30)]
        public string? StaffId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = UserRoles.Customer;

        // <summary>
        // Indicates whether the user has completed password setup.
        // </summary>

        public bool IsActive { get; set; } = false;

        // ==================
        // PASSWORD SETUP / INVITATION
        // ==================

        // <summary>
        // One-time token used by invited admins to set their password.
        // </summary>
        public string? PasswordSetupToken { get; set; }

        public DateTime? PasswordSetupTokenExpiresAt { get; set; }

        public string? PasswordResetToken { get; set; }

        public string? PasswordResetOtp { get; set;  }

        public DateTime? PasswordResetOtpExpiresAt { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime? LastLoginAt { get; set; }

        // =============
        // OTP / REGISTRATION VERIFICATION
        // =================

        // <summary>
        // One-time password used to verify customer registration.
        // </summary>
        [MaxLength(10)]
        public string? RegistrationOtp {  get; set; }

        // Expiration time for the registration OTP.

        public DateTime? RegistrationOtpExpiresAt { get; set; }

        //Number of OTP verification attempts.

        public int OtpAttempts { get; set; }

        //Indicates whether the customer's registration has been verified.

        public bool IsRegistrationVerified { get; set; } = false;

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
