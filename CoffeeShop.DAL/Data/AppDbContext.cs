using Microsoft.EntityFrameworkCore;
using CoffeeShop.Models.Entities.Auth;
using CoffeeShop.Models.Entities.Catalog;
using CoffeeShop.Models.Entities.Inventory;
using CoffeeShop.Models.Entities.Sales;
using CoffeeShop.Models.Entities.System;

namespace CoffeeShop.DAL.Data
{
    public class AppDbContext : DbContext 
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductRecipe> ProductRecipes { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<StoreInventory> StoreInventories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<SystemAuditLog> SystemAuditLogs { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<InventoryCheck> InventoryChecks { get; set; }

        public DbSet<ShiftReport> ShiftReports { get; set; }
        //Kỹ thuật Data Seeding (không cần phải Insert dữ liệu mỗi khi xoá database)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Bước "lấy khung" của Bố 
            base.OnModelCreating(modelBuilder);
            // 1. Thiết lập khóa chính phức hợp cho bảng ProductRecipe
            modelBuilder.Entity<ProductRecipe>()
                        .HasKey(pr => new { pr.ProductId, pr.ItemId });
            // Nó sẽ tự tìm tất cả các class kế thừa IEntityTypeConfiguration trong project này và nạp vào.
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}