using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CoffeeShop.Models.Entities.Inventory; 

namespace CoffeeShop.DAL.Configurations
{
    public class StoreConfiguration : IEntityTypeConfiguration<Store>
    {
        public void Configure(EntityTypeBuilder<Store> builder)
        {
            builder.ToTable("Stores");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.StoreName).IsRequired().HasMaxLength(30);
            builder.Property(s => s.Address).IsRequired().HasMaxLength(100);
            builder.Property(s => s.Hotline).IsRequired().HasMaxLength(20);
            builder.HasMany(s => s.Orders)
                   .WithOne(o => o.Store)
                   .HasForeignKey(o => o.StoreId)
                   .OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(s => s.StoreInventories)
                   .WithOne(o => o.Store)
                   .HasForeignKey(o => o.StoreId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}