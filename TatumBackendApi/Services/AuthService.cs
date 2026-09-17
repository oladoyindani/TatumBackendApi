using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using TatumBackendApi.Auth;
using TatumBackendApi.Common.Constants;
using TatumBackendApi.Dtos;
using TatumBackendApi.DTOs;
using TatumBackendApi.Entities;
using TatumBackendApi.Repositories;
using TatumBackendApi.Responses;

namespace TatumBackendApi.Services
{
    public interface IAuthService
    {
        
        Task<ApiResponse<LoginResponseDto>> LoginAsync(
            LoginRequestDto request,
            CancellationToken ct = default
        );

        Task<ApiResponse<UserDto>> RegisterAsync(
            RegisterRequestDto request,
            CancellationToken ct = default
        );

        Task<ApiResponse<UserDto>> VerifyRegistrationOtpAsync(
            VerifyRegistrationOtpRequestDto request,
            CancellationToken ct = default
        );

        Task<ApiResponse<UserDto>> ResetPasswordStartAsync(
            ResetPasswordStartDto request,
            CancellationToken ct = default
        );

        Task<ApiResponse<UserDto>> ResetPasswordAsync(
            ResetPasswordDto request,
            CancellationToken ct = default
        );

        Task<ApiResponse<UserDto>> ResendRegistrationOtpAsync(
            ResendRegistrationOtpRequestDto request, CancellationToken ct = default);
        Task<ApiResponse<UserDto>> GetCurrentUserAsync(
            Guid userId, CancellationToken ct = default);

        Task<ApiResponse<UserDto>> InviteAdminAsync(
            AdminInviteRequestDto request, CancellationToken ct = default);
        Task<ApiResponse<UserDto>> SetPasswordAsync(
            SetPasswordRequestDto request, CancellationToken ct = default);

        Task<ApiResponse<object>> ChangePasswordAsync(
            Guid userId, ChangePasswordRequestDto request, CancellationToken ct = default);

    }

    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly JwtService _jwtService;
        private readonly JwtSettings _settings;
        private readonly INotificationService _notificationService;
        private readonly EmailSettings _emailSettings;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUserRepository userRepository, IConfiguration configuration,
            JwtService jwtService, IOptions<EmailSettings> emailOptions, 
            IAccountRepository accountRepository,
            IOptions<JwtSettings> options, INotificationService notificationService
        )
        {
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _jwtService = jwtService;
            _settings = options.Value;
            _notificationService = notificationService;
            _emailSettings = emailOptions.Value;
            _configuration = configuration;
        }

        public async Task<ApiResponse<UserDto>> RegisterAsync(RegisterRequestDto request, CancellationToken ct = default)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            var phone = request.Phone.Trim();

            // VALIDATION

            if ( string.IsNullOrWhiteSpace(email))
            {
                return ApiResponse<UserDto>.Fail(
                    "Email is required.",
                    new List<ApiError>
                    {
                        new(
                            "InvalidEmail",
                            "Email is required."
                        )
                    }
                );
            }

            if (string.IsNullOrWhiteSpace(phone))
            {
                return ApiResponse<UserDto>.Fail(
                    "Phone number is required",
                    new List<ApiError>
                    {
                        new(
                            "InvalidPhone",
                            "Phone number is required."
                        )
                    }
                );
            }
            
            // CHECK EXISTING USER

            var existingUser = await _userRepository.GetByEmailAsync(email);

            if (existingUser != null)
            {
                // Allow an unverified registration to request
                // another OTP instead of creting another user.
                if (!existingUser.IsRegistrationVerified)
                {
                    return ApiResponse<UserDto>.Fail(
                        "Registration is already pending verification. ",
                        new List<ApiError>
                        {
                            new(
                                "RegistrationPending",
                                "A registration already exists for this email. Please verify the OTP."
                            )
                        }
                    );
                }

                return ApiResponse<UserDto>.Fail(
                    "An account with this email already exists.",
                    new List<ApiError>
                    {
                        new(
                            "EmailExists",
                            "An account with this email already exists."
                        )
                    }
                );
            }

            // GENERATE OTP

            var otp = GenerateOtp();

            var user = new Entities.User
            {
                Id = Guid.NewGuid(),
                Email = email,
                Phone = phone,
                FirstName = request.FirstName?.Trim(),
                LastName = request.LastName?.Trim(),
                PasswordHash = HashPassword(request.Password),
                Role = UserRoles.Customer,

                //IMPORTANT
                // Customer is not active until OTP veriication.
                IsActive = false,

                IsRegistrationVerified = false, 

                RegistrationOtp = otp,
                RegistrationOtpExpiresAt = DateTime.UtcNow.AddMinutes(10),
                OtpAttempts = 0,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            await _userRepository.SaveChangesAsync(ct);

            // SEND OTP

            await SendRegistrationOtpAsync(
                user,
                otp,
                ct
            );

            // RESPONSE

            return ApiResponse<UserDto>.Ok(
                MapToDto(user),
                "Registration initiated successfully." +
                "A verification code has been sent to your email and phone."
            );
        }

        public static string GenerateOtp()
        {
            return RandomNumberGenerator
                .GetInt32(10000, 1000000).ToString();
        }

        private async Task SendRegistrationOtpAsync(
            Entities.User user,
            string otp,
            CancellationToken ct
        )
        {
            var subject = "TatumConnect Registration Verification";
            var message = $"""
                Hello {user.FirstName},
                Welcome to TatumBank.
                your registration verification code is: 
                {otp}
                This code expires in 10 minutes.
                If you did not initiate this registration, please ignore this message.
                Regards,
                TatumBankOladoyin
                """;

                // EMAIL

                await _notificationService.SendEmailAsync(
                    user.Email,
                    subject,
                    message,
                    ct
                );

                // PHONE / SMS

                //await _notificationService.SendSmsAsync(
                //  user.Phone!,
                //  $"Your TatumBank verification code is {otp}. It expires in 10 minutes.",
                //  ct);
        }

        // public async Task<ApiResponse<UserDto>>

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();

            var bytes = Encoding.UTF8.GetBytes(password);

            var hash = sha.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }

        private static bool VerifyPassword(string password, string? passwordHash)
        {
            return !string.IsNullOrWhiteSpace(passwordHash)
                && HashPassword(password) == passwordHash;
        }

        // INVALID CREDENTIALS


        // DTO MAPPING

        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Department = user.Department,
                ProfileImageUrl = user.ProfileImageUrl,
                StaffId = user.StaffId,
                Role = user.Role,
                RegistrationOtp = user.RegistrationOtp,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            };
        }

        private async Task SendResetPasswordOtpAsync(Entities.User user, string otp, CancellationToken ct)
        {
            var subject = "TatumConnect Password Reset Verification";

            var message = $"""

                Hello {user.FirstName},

                We received a request to reset your TatumConnect password.

                Your password reset code is:

                {otp}

                This code expires in 10 minutes.

                If you did not request a password reset, please ignore this message.

                Regards,
                TatumConnect
                """;

            var emailResult = await _notificationService.SendEmailAsync(
                user.Email,
                subject,
                message,
                ct
            );

            if (!emailResult.Success)
            {
                Console.WriteLine($"Email send failed: {string.Join(", ", emailResult.Errors?.Select(e => e.Message) ?? new List<string>())}");
            }
        }

        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request?.Email))
            {
                return ApiResponse<LoginResponseDto>.Fail(
                    "Email is required.",
                    new List<ApiError> { new("InvalidEmail", "Email is required.") }
                );
            }

            if (string.IsNullOrWhiteSpace(request?.Password))
            {
                return ApiResponse<LoginResponseDto>.Fail(
                    "Password is required.",
                    new List<ApiError> { new("InvalidPassword", "Password is required.") }
                );
            }

            var email = request.Email.Trim().ToLowerInvariant();
            var password = request.Password.Trim();

            // Fetch User

            var user = await _userRepository.GetByEmailAsync(email);

            // Generic error message prevents username enumeration attacks
            if (user == null || !VerifyPassword(password, user.PasswordHash))
            {
                return ApiResponse<LoginResponseDto>.Fail(
                    "Invalid email or password.",
                    new List<ApiError> { new("InvalidCredentials", "Invalid email or password.") }
                );
            }

            // Check account status

            // if (!user.IsRegistrationVerified)
            // {
            //     return ApiResponse<LoginResponseDto>.Fail(
            //         "Account verification is pending.",
            //         new List<ApiError> { new("AccountUnverified", "Please verify your email/phone before logging in.") }
            //     );
            // }

            if (!user.IsActive)
            {
                return ApiResponse<LoginResponseDto>.Fail(
                    "Account is inactive.",
                    new List<ApiError> { new("AccountInactive", "Your account has been deactivated.") }
                );
            }

            // Update lastLogin

            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.SaveChangesAsync(ct);

            // Response for succesful login

            return ApiResponse<LoginResponseDto>.Ok(
                new LoginResponseDto
                {
                    AccessToken = _jwtService.GenerateAccessToken(user),
                    ExpiresInSeconds = _settings.AccessTokenMinutes * 60,
                    User = MapToDto(user)
                },
                "Login successful."
            );
        }

        public async Task<ApiResponse<UserDto>> VerifyRegistrationOtpAsync(VerifyRegistrationOtpRequestDto request, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request?.Email))
            {
                return ApiResponse<UserDto>.Fail(
                    "Email is required.",
                    new List<ApiError> { new("InvalidEmail", "Email is required.") }
                );
            }

            if (string.IsNullOrWhiteSpace(request.Otp))
            {
                return ApiResponse<UserDto>.Fail(
                    "OTP is required.",
                    new List<ApiError> { new("InvalidOtp", "OTP is required.") }
                );
            }

            var email = request.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null || user.IsRegistrationVerified)
            {
                return ApiResponse<UserDto>.Fail(
                    "Invalid or expired verification code.",
                    new List<ApiError> { new("InvalidOtp", "Invalid or expired verification code.") }
                );
            }

            if (user.RegistrationOtp != request.Otp.Trim()
                || user.RegistrationOtpExpiresAt is null
                || user.RegistrationOtpExpiresAt <= DateTime.UtcNow)
            {
                return ApiResponse<UserDto>.Fail(
                    "Invalid or expired verification code.",
                    new List<ApiError> { new("InvalidOtp", "Invalid or expired verification code.") }
                );
            }

            user.IsRegistrationVerified = true;
            user.IsActive = true;
            user.RegistrationOtp = null;
            user.RegistrationOtpExpiresAt = null;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.SaveChangesAsync(ct);

            return ApiResponse<UserDto>.Ok(
                MapToDto(user),
                "Registration verified successfully."
            );
        }

        private async Task<string> GenerateAccountNumberAsync()
        {
            while (true)
            {
                var accountNumber = RandomNumberGenerator.GetInt32(10000000, 100000000).ToString();

                // var exists = await _accountRepository
            }
        }

        public async Task<ApiResponse<UserDto>> ResetPasswordStartAsync(ResetPasswordStartDto request, CancellationToken ct = default)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            var existingUser = await _userRepository.GetByEmailAsync(email);

            if (existingUser == null)
            {
                return ApiResponse<UserDto>.Fail(
                    "User not found",
                    new List<ApiError>
                    {
                        new("UserNotFound", "No user exists for this email")
                    });
            }

            //if (!existingUser.IsRegistrationVerified)
            //{
            //    return ApiResponse<UserDto>.Fail(
            //        "Registration pending",
            //        new List<ApiError>
            //        {
            //            new("RegistrationPending", "Please complete registration before resetting your password")
            //        });
            //}

            var otp = GenerateOtp();

            existingUser.PasswordResetOtp = otp;
            existingUser.PasswordResetOtpExpiresAt = DateTime.UtcNow.AddMinutes(10);

            await _userRepository.UpdateAsync(existingUser);
            await _userRepository.SaveChangesAsync(ct);
            await SendResetPasswordOtpAsync(existingUser, otp, ct);

            return ApiResponse<UserDto>.Ok(null!, "Reset code sent to email");
        }

        public async Task<ApiResponse<UserDto>> ResetPasswordAsync(ResetPasswordDto request, CancellationToken ct = default)
        {
            var email = request.Email.Trim().ToLowerInvariant();
            var otp = request.Otp.Trim();
            var newPassword = request.NewPassword;
            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser == null)
            {
                return ApiResponse<UserDto>.Fail(
                    "User not found",
                    new List<ApiError>
                    {
                        new("UserNotFound", "No user exists for this email")
                    });
            }
            if (existingUser.PasswordResetOtp == null || existingUser.PasswordResetOtpExpiresAt < DateTime.UtcNow)
            {
                return ApiResponse<UserDto>.Fail(
                    "Invalid or expired OTP",
                    new List<ApiError>
                    {
                        new("InvalidOrExpiredOTP", "The provided OTP is invalid or has expired")
                    });
            }
            if (existingUser.PasswordResetOtp != otp)
            {
                return ApiResponse<UserDto>.Fail(
                    "Invalid OTP",
                    new List<ApiError>
                    {
                        new("InvalidOTP", "The provided OTP is invalid")
                    });
            }
            existingUser.PasswordHash = HashPassword(newPassword);
            existingUser.PasswordResetOtp = null;
            existingUser.PasswordResetOtpExpiresAt = null;
            await _userRepository.UpdateAsync(existingUser);
            await _userRepository.SaveChangesAsync(ct);
            return ApiResponse<UserDto>.Ok(MapToDto(existingUser), "Password reset successfully");
        }

        public async Task<ApiResponse<object>> ChangePasswordAsync(
            Guid userId,
            ChangePasswordRequestDto request,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.CurrentPassword) ||
                string.IsNullOrWhiteSpace(request.NewPassword) ||
                string.IsNullOrWhiteSpace(request.ConfirmNewPassword))
            {
                return ApiResponse<object>.Fail(
                    "All fields are required.",
                    new List<ApiError> { new("InvalidRequest", "All fields are required.") });
            }

            if (request.NewPassword != request.ConfirmNewPassword)
            {
                return ApiResponse<object>.Fail(
                    "Passwords do not match.",
                    new List<ApiError> { new("PasswordMismatch", "Passwords do not match.") });
            }

            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return ApiResponse<object>.Fail(
                    "User not found.",
                    new List<ApiError> { new("UserNotFound", "User not found.") });
            }

            if (!VerifyPassword(request.CurrentPassword, user.PasswordHash))
            {
                return ApiResponse<object>.Fail(
                    "Current password is incorrect.",
                    new List<ApiError> { new("InvalidPassword", "Current password is incorrect.") });
            }

            if (VerifyPassword(request.NewPassword, user.PasswordHash))
            {
                return ApiResponse<object>.Fail(
                    "New password must be different from the current password.",
                    new List<ApiError> { new("SamePassword", "New password must be different from the current password.") });
            }

            user.PasswordHash = HashPassword(request.NewPassword);

            await _userRepository.SaveChangesAsync(ct);

            var response = new LoginResponseDto
            {
                //Token = token,
                User = MapToDto(user)
            };

            return ApiResponse<object>.Ok(response, "Password changed successfully.");
        }

        public async Task<ApiResponse<UserDto>> ResendRegistrationOtpAsync(ResendRegistrationOtpRequestDto request, CancellationToken ct = default)
        {
            var email = request.Email?.Trim()?.ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(email))
            {
                return ApiResponse<UserDto>.Fail("Email is required", new List<ApiError> { new("InvalidEmail", "Email is required") });
            }

            var user = await _userRepository.GetByEmailAsync(email);

            // Generic response to avoid leaking whether the email is registered
            if (user == null)
            {
                return ApiResponse<UserDto>.Fail(
                    "Unable to resend OTP",
                    new List<ApiError> { new("InvalidRequest", "Unable to process this request.") });
            }

            if (user.IsRegistrationVerified)
            {
                return ApiResponse<UserDto>.Fail(
                    "Account already verified",
                    new List<ApiError> { new("AlreadyVerified", "This account has already been verified.") });
            }

            const int maxOtpAttempts = 10;
            const int lockoutHours = 1;

            var now = DateTime.UtcNow;

            if (user.OtpAttempts >= maxOtpAttempts && user.RegistrationOtpExpiresAt.HasValue)
            {
                // "Issued at" = ExpiresAt - 10 minutes, same convention as before
                var issuedAt = user.RegistrationOtpExpiresAt.Value.AddMinutes(-10);
                var lockoutEndsAt = issuedAt.AddHours(lockoutHours);

                if (now < lockoutEndsAt)
                {
                    var waitMinutes = Math.Max(1, (int)Math.Ceiling((lockoutEndsAt - now).TotalMinutes));

                    return ApiResponse<UserDto>.Fail(
                        "Too many OTP requests",
                        new List<ApiError> { new("ResendLimitExceeded", $"Please wait {waitMinutes} minute(s) before requesting a new OTP.") });
                }

                // Lockout window has passed — reset the counter
                user.OtpAttempts = 0;
            }

            var otp = GenerateOtp();
            user.RegistrationOtp = otp;
            user.RegistrationOtpExpiresAt = now.AddMinutes(10);
            user.OtpAttempts++;

            await _userRepository.SaveChangesAsync(ct);

            await SendRegistrationOtpAsync(user, otp, ct);

            return ApiResponse<UserDto>.Ok(
                MapToDto(user),
                "A new verification code has been sent to your email.");
        }

        public async Task<ApiResponse<UserDto>> GetCurrentUserAsync(Guid userId, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return ApiResponse<UserDto>.Fail(
                    "User not found",
                    new List<ApiError> { new("UserNotFound", "No user found for the current session.") });
            }

            var accounts = await _accountRepository.GetFilteredAsync(customerId: user.Id);

            return ApiResponse<UserDto>.Ok(
                MapToDto(user),
                "User retrieved successfully.");
        }


        private static readonly HashSet<string> InvitableRoles = new()
        {
            UserRoles.SuperAdmin, UserRoles.Admin, UserRoles.Staff, UserRoles.Manager
        };

        public async Task<ApiResponse<UserDto>> InviteAdminAsync(AdminInviteRequestDto request, CancellationToken ct = default)
        {
            var email = request.Email.Trim().ToLowerInvariant();
            var phone = request.Phone.Trim();
            var role = request.Role.Trim();

            if (!InvitableRoles.Contains(role))
            {
                return ApiResponse<UserDto>.Fail(
                    "Invalid role specified.",
                    new List<ApiError>
                    {
                        new("InvalidRole", $"Role must be one of: {string.Join(", ", InvitableRoles)}")
                    });
            }

            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
            {
                return ApiResponse<UserDto>.Fail(
                    "An account with this email already exists",
                    new List<ApiError>
                    {
                        new("EmailExists", "An account with this email already exists.")
                    });
            }

            var setupToken = _jwtService.GenerateRefreshToken();

            var user = new Entities.User
            {
                Id = Guid.NewGuid(),
                Email = email,
                Phone = phone,
                FirstName = request.FirstName?.Trim(),
                LastName = request.LastName?.Trim(),
                Department = request.Department?.Trim(),
                Role = role,
                IsActive = false,
                IsRegistrationVerified = true,
                PasswordSetupToken = setupToken,
                PasswordSetupTokenExpiresAt = DateTime.UtcNow.AddHours(24),
                CreatedAt = DateTime.UtcNow,
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync(ct);

            await SendAdminInviteEmailAsync(user, setupToken, ct);

            return ApiResponse<UserDto>.Ok(
                MapToDto(user),
                "Invitation sent successfully.");
        }

        public async Task<ApiResponse<UserDto>> SetPasswordAsync(SetPasswordRequestDto request, CancellationToken ct = default)
        {
            if (request.Password != request.ConfirmPassword)
            {
                return ApiResponse<UserDto>.Fail(
                    "Passwords do not match.",
                    new List<ApiError>
                    {
                        new("PasswordMismatch", "Password and confirmation password do not match.")
                    });
            }

            var user = await _userRepository.GetByPasswordSetupTokenAsync(request.Token);

            if (user == null)
            {
                return ApiResponse<UserDto>.Fail(
                    "Invalid or expired token.",
                    new List<ApiError>
                    {
                        new("InvalidToken", "This invitation link is invalid or has expired.")
                    });
            }

            user.PasswordHash = HashPassword(request.Password);
            user.IsActive = true;
            user.PasswordSetupToken = null;
            user.PasswordSetupTokenExpiresAt = null;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.SaveChangesAsync(ct);

            return ApiResponse<UserDto>.Ok(
                MapToDto(user),
                "Password set successfully. You can now log in.");
        }

        private async Task SendAdminInviteEmailAsync(Entities.User user, string token, CancellationToken ct)
        {
            var subject = "You've been invited to TatumConnect";
            var setupLink = $"{_emailSettings.FrontendBaseUrl}/change-password?token={Uri.EscapeDataString(token)}";
            var message = $"""
                Hello {user.FirstName},

                You have been invited to join TatumConnect as a {user.Role}.

                Click the link below to set your password and activate your account:

                {setupLink}

                This link expires in 24 hours.

                If you were not expecting this invitation, please ignore this message.

                Regards.
                TatumConnect
                """;

            await _notificationService.SendEmailAsync(
                user.Email,
                subject,
                message,
                ct);
        }
    }

}