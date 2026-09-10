using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CoffeeShop.Models.Entities.Auth;


namespace CoffeeShop.DAL.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Email).IsRequired().HasMaxLength(255).IsUnicode(false);
            builder.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
            builder.Property(e => e.Role).IsRequired().HasMaxLength(10).HasDefaultValue("Staff");
            builder.Property(e => e.FailedLoginAttempts).HasDefaultValue(0);
            builder.Property(e => e.LockoutEnd).IsRequired(false); 
            builder.Property(e => e.OtpCode).IsRequired();
            builder.Property(e => e.OtpExpiryTime).HasColumnType("timestamp with time zone");
            builder.HasOne(u => u.Store)
                .WithMany(s => s.Users)
                .HasForeignKey(u => u.StoreId)
                .OnDelete(DeleteBehavior.Restrict);
            //Cơ chế của EF Core rất thông minh, khi đã cấu hình 1-1 hay 1-n ở 1 trong 2 bảng
            //Nó sẽ ngay lập tức hiểu 2 bảng này có mối quan hệ => Không cần cấu hình nhiều
        }
    }
}