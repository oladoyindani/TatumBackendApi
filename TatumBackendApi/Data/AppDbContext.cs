using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using TatumBackendApi.Entities;

namespace TatumBackendApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Account> Accounts => Set<Account>();
        //public DbSet<Transaction> Transactions => Set<Transactions>();
        //public DbSet<Transfer> Transfers => Set<Transfer>();
        //idempotency removed per new simplified architecture
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        //public DbSet<Order> Orders => Set<Order>();
        //public DbSet<Product> Products {  get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

            modelBuilder.Entity<Account>().HasKey(a => a.Id);
            //modelBuilder.Entity<Transaction>().HasKey(a => a.Id);

            // idempotency index removed
            modelBuilder.Entity<RefreshToken>().HasKey(r => r.Id);
        }
    }
}
