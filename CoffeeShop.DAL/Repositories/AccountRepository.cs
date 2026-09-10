using Microsoft.EntityFrameworkCore;
using CoffeeShop.Models.Entities.Auth;
using CoffeeShop.DAL.Data;

namespace CoffeeShop.DAL.Repositories
{
    public class AccountRepository
    {
        private readonly AppDbContext _context;
        
        public AccountRepository(AppDbContext context) {
            _context = context;
        }

        // HÀM 1: Tìm con mồi
        public async Task<User> FindByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        // HÀM 2: Cập nhật mật khẩu
        public async Task<bool> UpdateAccountPasswordAsync(string email, string newPasswordHash)
        {
            // Bước 1: Tìm kiếm
            var user = await FindByEmailAsync(email);
            if (user == null) 
            {
                return false;
            }

            // Bước 2: Thay mật khẩu mới
            user.PasswordHash = newPasswordHash;
            user.OtpCode = "000000";

            // Bước 3: CHỈ GỌI LỆNH LƯU XUỐNG DB Ở ĐÂY
            await _context.SaveChangesAsync();
            
            return true;
        }

        // HÀM 3: Nút bấm "Lưu" để cho các Service khác (như RecoveryService) gọi từ bên ngoài vào
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}