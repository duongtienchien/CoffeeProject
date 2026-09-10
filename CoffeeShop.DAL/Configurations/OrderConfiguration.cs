using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CoffeeShop.Models.Entities.Sales;

namespace CoffeeShop.DAL.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.CreateDate).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(e => e.TotalAmount).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(e => e.PaymentMethod).IsRequired().HasMaxLength(50);
            // Mapping quan hệ 1-N chuẩn sách giáo khoa
            builder.HasMany(o => o.OrderDetails)
                   .WithOne(od => od.Order) // Điền rõ navigation property vào đây
                   .HasForeignKey(od => od.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);
            // Một đơn hàng do MỘT nhân viên (User) tạo ra
            builder.HasOne(o => o.Staff) 
                   .WithMany()
                   .HasForeignKey(o => o.StaffId)
                   .OnDelete(DeleteBehavior.Restrict);
        }

    }
}