using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CoffeeShop.Models.Entities.Catalog;

namespace CoffeeShop.DAL.Configurations
{
    public class ProductRecipeConfiguration : IEntityTypeConfiguration<ProductRecipe>
    {
        public void Configure(EntityTypeBuilder<ProductRecipe> builder)
        {
            builder.ToTable("ProductRecipes");
            builder.HasKey(e => new { e.ProductId, e.ItemId });
            builder.Property(e => e.QuantityNeeded).IsRequired();
        }
    }
}