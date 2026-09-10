using Microsoft.EntityFrameworkCore;
using CoffeeShop.Models.Entities.Auth;
using CoffeeShop.Models.Entities.Inventory;
using CoffeeShop.Models.Entities.Catalog;
using CoffeeShop.DAL.Data;

namespace CoffeeShop.DAL.Repositories
{
    public class StaffRepository
    {
        private readonly AppDbContext _dbContext;
        public StaffRepository (AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<User> GetUserByIdAsync (int id) 
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        }
        public async Task<StoreInventory> GetInventoryByIdAsync (int id)
        {
            return await _dbContext.StoreInventories.FirstOrDefaultAsync(s => s.ItemId == id);
        }
        public async Task<Product> GetProductByIdAsync (int id)
        {
            return await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<bool> SaveChangesAsync()
        {
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }
    }
}