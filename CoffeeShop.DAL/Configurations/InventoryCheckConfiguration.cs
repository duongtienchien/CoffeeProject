using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CoffeeShop.Models.Entities.Inventory;

namespace CoffeeShop.DAL.Configurations
{
    public class InventoryCheckConfiguration : IEntityTypeConfiguration<InventoryCheck>
    {
        public void Configure(EntityTypeBuilder<InventoryCheck> builder)
        {
            // 1. Cấu hình Khóa chính (Thay cho [Key])
            builder.HasKey(x => x.Id);

            // 2. Cấu hình kiểu dữ liệu (Thay cho [Column(TypeName="...")])
            builder.Property(x => x.SystemQuantity).HasColumnType("decimal(18,2)");
            builder.Property(x => x.ActualQuantity).HasColumnType("decimal(18,2)");
            builder.Property(x => x.Discrepancy).HasColumnType("decimal(18,2)");
            builder.Property(x => x.Explanation).HasMaxLength(500);

            // 3. Cấu hình Quan hệ (Foreign Keys)
            // Một phiếu kiểm kho thuộc về 1 Cửa hàng
            builder.HasOne(x => x.Store)
                   .WithMany() // Nếu Store không chứa List<InventoryCheck> thì để trống WithMany()
                   .HasForeignKey(x => x.StoreId)
                   .OnDelete(DeleteBehavior.Restrict); // Không cho phép xóa Cửa hàng nếu đang có phiếu kiểm kho

            // Một phiếu kiểm kho chỉ kiểm tra 1 Nguyên liệu
            builder.HasOne(x => x.InventoryItem)
                   .WithMany()
                   .HasForeignKey(x => x.ItemId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Một phiếu kiểm kho do 1 Người quản lý thực hiện
            builder.HasOne(x => x.Manager)
                   .WithMany()
                   .HasForeignKey(x => x.ManagerId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Tùy chọn: Link 1-1 với cái Transaction dùng để bù trừ kho sau này
            builder.HasOne(x => x.ResolvedTransaction)
                   .WithMany()
                   .HasForeignKey(x => x.ResolvedTransactionId)
                   .OnDelete(DeleteBehavior.SetNull); 
        }
    }
}