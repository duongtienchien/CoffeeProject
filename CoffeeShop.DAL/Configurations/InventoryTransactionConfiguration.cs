using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CoffeeShop.Models.Entities.Inventory;


namespace CoffeeShop.DAL.Configurations
{
    public class InventoryTransactionConfiguration : IEntityTypeConfiguration<InventoryTransaction>
    {
        public void Configure(EntityTypeBuilder<InventoryTransaction> builder)
        {
            builder.ToTable("InventoryTransactions");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.QuantityChange).IsRequired();
            builder.Property(e => e.TransactionType).IsRequired().HasMaxLength(100);
            builder.Property(e => e.Reason).IsRequired().HasMaxLength(200);
            builder.Property(e => e.CreateBy).IsRequired();
            //Liên kết 1 Store nhiều InventoryTransactions
            builder.HasOne(e => e.Store)
                   .WithMany(e => e.InventoryTransactions)
                   .HasForeignKey(e => e.StoreId);
            //Liên kết 1 InventoryItem nhiều InventoryTransactions
            builder.HasOne(e => e.InventoryItem)
                   .WithMany(e => e.InventoryTransactions)
                   .HasForeignKey(e => e.ItemId);
            // Giao dịch kho do MỘT nhân viên thực hiện
            builder.HasOne(it => it.User) 
                   .WithMany()
                   .HasForeignKey(it => it.CreateBy) // Mapping đích danh khóa ngoại là CreateBy
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}