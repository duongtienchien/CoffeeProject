using CoffeeShop.DAL.Interfaces; 
using CoffeeShop.DAL.Data;
using CoffeeShop.Models.Entities.Auth;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShop.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;
        public UserRepository (AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<User> GetUserByEmail(string email)
        {
            // Nhét ngay Include vào đây để kéo theo cục Profile lên!
            return await _dbContext.Users
                .Include(u => u.UserProfile) 
                .FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<List<User>> GetAllStaffAsync()
        {
            return await _dbContext.Users
                .Include(u => u.UserProfile)   // Móc luôn bảng Profile sang để lát còn lấy Avatar, SĐT
                .Where(u => u.Role.ToLower() == "staff") // Lọc ra những ông nào làm Staff
                .ToListAsync();
        }
    }
}