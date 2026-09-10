using Microsoft.EntityFrameworkCore;
using CoffeeShop.Models.Entities.Catalog;
using CoffeeShop.DAL.Data;
namespace CoffeeShop.DAL.Repositories
{
    public class ProductRepository
    {
        private readonly AppDbContext _dbContext;
        public ProductRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Product> GetProductByIdAsync(int Id)
        {
            return await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == Id);
        }
        // --- HÀM MỚI TOANH ĐỂ LẤY TOÀN BỘ MENU ---
        public async Task<List<Product>> GetAllProductsWithCategoryAsync()
        {
            return await _dbContext.Products
                .Include(p => p.Category) // Lôi thằng Category đi theo để lấy tên nhóm cà phê
                .ToListAsync();
        }
    }
}