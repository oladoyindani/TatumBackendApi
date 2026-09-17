using Microsoft.EntityFrameworkCore;
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
        public DbSet<Transaction> Transactions => Set<Transaction>();
        //public DbSet<Transfer> Transfers => Set<Transfer>();
        //idempotency removed per new simplified architecture
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        //public DbSet<Order> Orders => Set<Order>();
        //public DbSet<Product> Products {  get; set; }

        // Billing / product entities
        public DbSet<Biller> Billers => Set<Biller>();
        public DbSet<Product> Products => Set<Product>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

            modelBuilder.Entity<Account>().HasKey(a => a.Id);
            //modelBuilder.Entity<Transaction>().HasKey(a => a.Id);

            // idempotency index removed
            modelBuilder.Entity<RefreshToken>().HasKey(r => r.Id);

            // Transaction
            modelBuilder.Entity<Transaction>().HasKey(t => t.Id);

            // Biller
            modelBuilder.Entity<Biller>().HasKey(b => b.Id);
            modelBuilder.Entity<Biller>().Property(b => b.Category).HasConversion<string>();
            modelBuilder.Entity<Biller>().HasIndex(b => b.Code).IsUnique();

            // Product
            modelBuilder.Entity<Product>().HasKey(p => p.Id);
            modelBuilder.Entity<Product>().HasIndex(p => p.Code).IsUnique();
            modelBuilder.Entity<Product>().Property(p => p.Category).HasConversion<string>();
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Biller)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.BillerId)
                .OnDelete(DeleteBehavior.Cascade);

            // ProductItem
            modelBuilder.Entity<ProductItem>().HasKey(pi => pi.Id);
            modelBuilder.Entity<ProductItem>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.ProductItems)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

        }
        public DbSet<ProductItem> ProductItems => Set<ProductItem>();
    }
}
