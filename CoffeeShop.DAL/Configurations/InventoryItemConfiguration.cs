using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CoffeeShop.Models.Entities.Catalog;

namespace CoffeeShop.DAL.Configurations
{
    public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
    {
        public void Configure(EntityTypeBuilder<InventoryItem> builder)
        {
            builder.ToTable("InventoryItems");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name).IsRequired().HasMaxLength(50);
            builder.Property(e => e.Unit).IsRequired().HasMaxLength(20);
            builder.HasMany(p => p.ProductRecipes)
                   .WithOne(i => i.InventoryItem)
                   .HasForeignKey(p => p.ItemId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}