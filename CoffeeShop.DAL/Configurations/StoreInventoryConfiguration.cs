using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CoffeeShop.Models.Entities.Inventory;

namespace CoffeeShop.DAL.Configurations
{
    public class StoreInventoryConfiguration : IEntityTypeConfiguration<StoreInventory>
    {
        public void Configure(EntityTypeBuilder<StoreInventory> builder)
        {
            builder.ToTable("StoreInventories");
            builder.HasKey(si => new { si.StoreId, si.ItemId });
            builder.Property(e => e.ItemId).IsRequired();
            builder.Property(e => e.Quantity).IsRequired();
            builder.HasOne(si => si.Store)
                   .WithMany(s => s.StoreInventories)
                   .HasForeignKey(si => si.StoreId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(si => si.InventoryItem)
                   .WithMany()
                   .HasForeignKey(si => si.ItemId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}