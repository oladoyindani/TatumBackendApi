using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserRoles = TatumBackendApi.Common.Constants.UserRoles;
using TatumBackendApi.Entities;

namespace TatumBackendApi.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            await EnsureDatabaseReadyAsync(context);

            if (await context.Users.AnyAsync())
            {
                await SeedDemoDataAsync(context);
                return;
            }

            await SeedSuperAdminAsync(context);
            await SeedDemoDataAsync(context);
        }

        private static async Task EnsureDatabaseReadyAsync(AppDbContext context)
        {
            if (await RequiredTablesMissingAsync(context))
            {
                try
                {
                    await context.Database.EnsureDeletedAsync();
                }
                catch
                {
                    // Ignore cleanup errors if the database is already in a bad state.
                }

                await context.Database.EnsureCreatedAsync();
                return;
            }

            try
            {
                await context.Database.MigrateAsync();
            }
            catch
            {
                // Local/dev databases can be stale or partially initialized.
                // Recreate the schema so the project still boots correctly for demo work.
                try
                {
                    await context.Database.EnsureDeletedAsync();
                }
                catch
                {
                    // Ignore cleanup errors if the database is already in a bad state.
                }

                await context.Database.EnsureCreatedAsync();
            }
        }

        private static async Task<bool> RequiredTablesMissingAsync(AppDbContext context)
        {
            try
            {
                var tables = await context.Database.SqlQueryRaw<string>("SHOW TABLES").ToListAsync();
                var tableNames = tables
                    .Select(name => name.Trim())
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                var requiredTables = new[]
                {
                    "Users",
                    "Accounts",
                    "RefreshTokens",
                    "Billers",
                    "Products",
                    "ProductItems",
                    "Transactions"
                };

                return requiredTables.Any(table => !tableNames.Contains(table));
            }
            catch
            {
                return true;
            }
        }

        private static async Task SeedDemoDataAsync(AppDbContext context)
        {
            await SeedCustomersAsync(context);
            await SeedBillersAndProductsAsync(context);
            await SeedTransactionsAsync(context);
        }

        private static async Task<string> GenerateStaffIdAsync(AppDbContext context)
        {
            var year = DateTime.UtcNow.Year;

            var lastStaffId = await context.Users
                .Where(u => u.StaffId != null && u.StaffId.StartsWith($"STF-{year}-"))
                .OrderByDescending(u => u.StaffId)
                .Select(u => u.StaffId)
                .FirstOrDefaultAsync();

            var nextNumber = 1;

            if (!string.IsNullOrWhiteSpace(lastStaffId))
            {
                var numberPart = lastStaffId.Split('-').Last();

                if (int.TryParse(numberPart, out var currentNumber))
                {
                    nextNumber = currentNumber + 1;
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
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow
            };

            await context.Users.AddAsync(superAdmin);
            await context.SaveChangesAsync();
        }

        private static async Task SeedCustomersAsync(AppDbContext context)
        {
            if (await context.Users.AnyAsync(u => u.Role == UserRoles.Customer))
            {
                return;
            }

            var customers = new[]
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    Email = "ada.okafor@tatumdemo.com",
                    PasswordHash = HashPassword("Customer@123"),
                    FirstName = "Ada",
                    LastName = "Okafor",
                    Phone = "+2348123456789",
                    Department = "Personal",
                    Role = UserRoles.Customer,
                    IsActive = true,
                    IsRegistrationVerified = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    UpdatedAt = DateTime.UtcNow.AddDays(-30),
                    LastLoginAt = DateTime.UtcNow.AddDays(-2)
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Email = "chinedu.adebayo@tatumdemo.com",
                    PasswordHash = HashPassword("Customer@123"),
                    FirstName = "Chinedu",
                    LastName = "Adebayo",
                    Phone = "+2348034567890",
                    Department = "Personal",
                    Role = UserRoles.Customer,
                    IsActive = true,
                    IsRegistrationVerified = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-25),
                    UpdatedAt = DateTime.UtcNow.AddDays(-25),
                    LastLoginAt = DateTime.UtcNow.AddDays(-3)
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Email = "mariam.ali@tatumdemo.com",
                    PasswordHash = HashPassword("Customer@123"),
                    FirstName = "Mariam",
                    LastName = "Ali",
                    Phone = "+2347056789012",
                    Department = "Personal",
                    Role = UserRoles.Customer,
                    IsActive = true,
                    IsRegistrationVerified = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-18),
                    UpdatedAt = DateTime.UtcNow.AddDays(-18),
                    LastLoginAt = DateTime.UtcNow.AddDays(-5)
                }
            };

            await context.Users.AddRangeAsync(customers);
            await context.SaveChangesAsync();

            var createdAccounts = new[]
            {
                new Account
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customers[0].Id,
                    AccountNumber = "1000000001",
                    Name = "Ada Okafor Main",
                    Currency = "NGN",
                    LedgerBalance = 350000m,
                    AvailableBalance = 320000m,
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    IsActive = true,
                    Customer = customers[0]
                },
                new Account
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customers[1].Id,
                    AccountNumber = "1000000002",
                    Name = "Chinedu Adebayo Savings",
                    Currency = "NGN",
                    LedgerBalance = 250000m,
                    AvailableBalance = 240000m,
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow.AddDays(-25),
                    IsActive = true,
                    Customer = customers[1]
                },
                new Account
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customers[2].Id,
                    AccountNumber = "1000000003",
                    Name = "Mariam Ali Wallet",
                    Currency = "NGN",
                    LedgerBalance = 180000m,
                    AvailableBalance = 175000m,
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow.AddDays(-18),
                    IsActive = true,
                    Customer = customers[2]
                }
            };

            await context.Accounts.AddRangeAsync(createdAccounts);
            await context.SaveChangesAsync();
        }

        private static async Task SeedBillersAndProductsAsync(AppDbContext context)
        {
            if (await context.Billers.AnyAsync())
            {
                return;
            }

            var billers = new[]
            {
                new Biller
                {
                    Id = Guid.NewGuid(),
                    Code = "MTN",
                    Name = "MTN Nigeria",
                    Category = BillerCategory.TeleCommunication,
                    Description = "Mobile telecommunication services",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-90)
                },
                new Biller
                {
                    Id = Guid.NewGuid(),
                    Code = "GLO",
                    Name = "Globacom",
                    Category = BillerCategory.TeleCommunication,
                    Description = "Mobile telecommunication services",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-90)
                },
                new Biller
                {
                    Id = Guid.NewGuid(),
                    Code = "DSTV",
                    Name = "DSTV",
                    Category = BillerCategory.CableTv,
                    Description = "Cable and satellite TV subscription",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-90)
                },
                new Biller
                {
                    Id = Guid.NewGuid(),
                    Code = "IKEJA",
                    Name = "Ikeja Electric",
                    Category = BillerCategory.Electricity,
                    Description = "Power utility bill payment",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-90)
                }
            };

            await context.Billers.AddRangeAsync(billers);
            await context.SaveChangesAsync();

            var mtn = billers[0];
            var glo = billers[1];
            var dstv = billers[2];
            var ikeja = billers[3];

            var products = new[]
            {
                new Product
                {
                    Id = Guid.NewGuid(),
                    BillerId = mtn.Id,
                    Code = "AIRTIME",
                    Name = "MTN Airtime",
                    Category = ProductCategory.Airtime,
                    Description = "Recharge airtime for mobile phones",
                    UnitPrice = 100m,
                    AllowsCustomAmount = true,
                    RequiresProductItem = false,
                    IsActive = true,
                    RequiredFields = JsonSerializer.Serialize(new { phoneNumber = "string" }),
                    CreatedAt = DateTime.UtcNow.AddDays(-80)
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    BillerId = mtn.Id,
                    Code = "DATA_7GB",
                    Name = "MTN Data 7GB",
                    Category = ProductCategory.Data,
                    Description = "Mobile data bundle",
                    UnitPrice = null,
                    AllowsCustomAmount = false,
                    RequiresProductItem = true,
                    IsActive = true,
                    RequiredFields = JsonSerializer.Serialize(new { phoneNumber = "string" }),
                    CreatedAt = DateTime.UtcNow.AddDays(-80)
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    BillerId = dstv.Id,
                    Code = "DSTV_COMPACT",
                    Name = "DSTV Compact",
                    Category = ProductCategory.CableTv,
                    Description = "Cable subscription package",
                    UnitPrice = 12000m,
                    AllowsCustomAmount = false,
                    RequiresProductItem = true,
                    IsActive = true,
                    RequiredFields = JsonSerializer.Serialize(new { smartCardNumber = "string" }),
                    CreatedAt = DateTime.UtcNow.AddDays(-80)
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    BillerId = ikeja.Id,
                    Code = "ELECTRICITY",
                    Name = "Ikeja Electric Bill",
                    Category = ProductCategory.Electricity,
                    Description = "Electricity bill payment",
                    UnitPrice = 5000m,
                    AllowsCustomAmount = true,
                    RequiresProductItem = false,
                    IsActive = true,
                    RequiredFields = JsonSerializer.Serialize(new { meterNumber = "string" }),
                    CreatedAt = DateTime.UtcNow.AddDays(-80)
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    BillerId = glo.Id,
                    Code = "GLO_DATA",
                    Name = "Glo Data",
                    Category = ProductCategory.Data,
                    Description = "Glo data bundle",
                    UnitPrice = null,
                    AllowsCustomAmount = false,
                    RequiresProductItem = true,
                    IsActive = true,
                    RequiredFields = JsonSerializer.Serialize(new { phoneNumber = "string" }),
                    CreatedAt = DateTime.UtcNow.AddDays(-80)
                }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();

            var mtnDataProduct = products[1];
            var dstvProduct = products[2];
            var gloDataProduct = products[4];

            var productItems = new[]
            {
                new ProductItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = mtnDataProduct.Id,
                    Code = "DATA_1GB_30D",
                    Name = "1GB - 30 Days",
                    Description = "1 gigabyte data bundle",
                    UnitPrice = 850m,
                    Quantity = 1m,
                    Unit = "GB",
                    ValidityDays = 30,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-70)
                },
                new ProductItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = mtnDataProduct.Id,
                    Code = "DATA_5GB_30D",
                    Name = "5GB - 30 Days",
                    Description = "5 gigabyte data bundle",
                    UnitPrice = 2500m,
                    Quantity = 5m,
                    Unit = "GB",
                    ValidityDays = 30,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-70)
                },
                new ProductItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = dstvProduct.Id,
                    Code = "DSTV_COMPACT_30D",
                    Name = "DSTV Compact",
                    Description = "Monthly compact bouquet",
                    UnitPrice = 12000m,
                    Quantity = 1m,
                    Unit = "Month",
                    ValidityDays = 30,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-70)
                },
                new ProductItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = gloDataProduct.Id,
                    Code = "GLO_2GB_30D",
                    Name = "2GB - 30 Days",
                    Description = "2 gigabyte data bundle",
                    UnitPrice = 1100m,
                    Quantity = 2m,
                    Unit = "GB",
                    ValidityDays = 30,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-70)
                }
            };

            await context.ProductItems.AddRangeAsync(productItems);
            await context.SaveChangesAsync();
        }

        private static async Task SeedTransactionsAsync(AppDbContext context)
        {
            if (await context.Transactions.AnyAsync())
            {
                return;
            }

            var customer = await context.Users
                .FirstOrDefaultAsync(u => u.Email == "ada.okafor@tatumdemo.com");

            if (customer == null)
            {
                return;
            }

            var account = await context.Accounts
                .FirstOrDefaultAsync(a => a.CustomerId == customer.Id);

            if (account == null)
            {
                return;
            }

            var mtnAirtime = await context.Products
                .FirstOrDefaultAsync(p => p.Code == "AIRTIME");

            var mtnData = await context.Products
                .FirstOrDefaultAsync(p => p.Code == "DATA_7GB");

            var dataItem = await context.ProductItems
                .FirstOrDefaultAsync(pi => pi.ProductId == mtnData!.Id);

            var transactions = new[]
            {
                new Transaction
                {
                    Id = Guid.NewGuid(),
                    AccountId = account.Id,
                    CustomerId = customer.Id,
                    Type = "Airtime",
                    Status = "Completed",
                    Reference = "TXN-20260917-1001",
                    Amount = 1500m,
                    Currency = "NGN",
                    Narration = "MTN airtime purchase",
                    BillerId = (await context.Billers.FirstAsync(b => b.Code == "MTN")).Id,
                    ProductId = mtnAirtime!.Id,
                    CustomerFields = "{\"phoneNumber\":\"08031234567\"}",
                    ProviderReference = "MTN-ABC123",
                    CreatedAt = DateTime.UtcNow.AddDays(-12),
                    CompletedAt = DateTime.UtcNow.AddDays(-12)
                },
                new Transaction
                {
                    Id = Guid.NewGuid(),
                    AccountId = account.Id,
                    CustomerId = customer.Id,
                    Type = "Data",
                    Status = "Completed",
                    Reference = "TXN-20260917-1002",
                    Amount = 2500m,
                    Currency = "NGN",
                    Narration = "MTN data bundle purchase",
                    BillerId = (await context.Billers.FirstAsync(b => b.Code == "MTN")).Id,
                    ProductId = mtnData!.Id,
                    ProductItemId = dataItem!.Id,
                    CustomerFields = "{\"phoneNumber\":\"08031234567\"}",
                    ProviderReference = "MTN-DATA-001",
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    CompletedAt = DateTime.UtcNow.AddDays(-5)
                },
                new Transaction
                {
                    Id = Guid.NewGuid(),
                    AccountId = account.Id,
                    CustomerId = customer.Id,
                    Type = "CableTv",
                    Status = "Pending",
                    Reference = "TXN-20260917-1003",
                    Amount = 12000m,
                    Currency = "NGN",
                    Narration = "DSTV Compact subscription",
                    BillerId = (await context.Billers.FirstAsync(b => b.Code == "DSTV")).Id,
                    ProductId = (await context.Products.FirstAsync(p => p.Code == "DSTV_COMPACT")).Id,
                    ProductItemId = (await context.ProductItems.FirstAsync(pi => pi.Code == "DSTV_COMPACT_30D")).Id,
                    CustomerFields = "{\"smartCardNumber\":\"1234567890\"}",
                    ProviderReference = null,
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                }
            };

            await context.Transactions.AddRangeAsync(transactions);
            await context.SaveChangesAsync();
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}