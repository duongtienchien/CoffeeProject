using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CoffeeShop.Models.Entities.Auth;

namespace CoffeeShop.DAL.Configurations
{
    public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.ToTable("UserProfiles");
            builder.HasKey(e => e.UserId);
            builder.Property(e => e.UserId).ValueGeneratedNever();
            builder.Property(e => e.FullName).IsRequired().HasMaxLength(50);
            builder.Property(e => e.Phone).IsRequired().HasMaxLength(20);
            builder.Property(e => e.Avatar).IsRequired().HasMaxLength(500);
            // Sửa thành như thế này: "Một UserProfile có MỘT User..."
            builder.HasOne(up => up.User)
                   // "...Và User đó có MỘT UserProfile"
                   .WithOne(u => u.UserProfile)
                   .HasForeignKey<UserProfile>(up => up.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
