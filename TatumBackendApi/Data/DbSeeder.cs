using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TatumBackendApi.Common.Constants;
using TatumBackendApi.Entities;

namespace TatumBackendApi.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // Apply pending migrations
            await context.Database.MigrateAsync();
            await SeedSuperAdminAsync(context);
            // await SeedBillersAndProductsAsync(context);
        }

        private static async Task<string> GenerateStaffIdAsync(AppDbContext context)
        {
            var year = DateTime.UtcNow.Year;

            var lastStaffId =
                await context.Users
                    .Where(u =>
                        u.StaffId != null &&
                        u.StaffId.StartsWith(
                            $"STF-{year}-"))
                    .OrderByDescending(
                        u => u.StaffId)
                    .Select(u => u.StaffId)
                    .FirstOrDefaultAsync();

            var nextNumber = 1;

            if (!string.IsNullOrWhiteSpace(lastStaffId))
            {
                var numberPart =
                    lastStaffId
                        .Split('-')
                        .Last();

                if (int.TryParse(
                    numberPart,
                    out var currentNumber))
                {
                    nextNumber =
                        currentNumber + 1;
                }
            }

            return $"STF-{year}-{nextNumber:D6}";
        }

        // Super Admin
        private static async Task SeedSuperAdminAsync(AppDbContext context)
        {
            const string email = "superadmin@tatumconnect.com";
            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (existingUser != null) return;

            var superAdmin = new User
            {
                Id = Guid.NewGuid(),
                Email = email,

                // Development password.
                // Change this immediately in a real environmen.
                PasswordHash = HashPassword("Admin@123456"),
                FirstName = "Tatum",
                LastName = "Super Admin",
                Phone = "+23480000000",
                Department = "Administration",
                ProfileImageUrl = null,
                StaffId = await GenerateStaffIdAsync(context),
                Role = UserRoles.SuperAdmin,
                IsActive = true,
                PasswordSetupToken = null,
                PasswordSetupTokenExpiresAt = null,
                CreatedAt = DateTime.UtcNow
                
            };

            await context.Users.AddAsync(superAdmin);
            await context.SaveChangesAsync();
        }

        // Password Hash

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create ();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}