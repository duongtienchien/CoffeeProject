using Microsoft.EntityFrameworkCore;
using CoffeeShop.Models.Entities.Catalog;
using CoffeeShop.DAL.Data;

namespace CoffeeShop.DAL.Repositories
{
    public class ProductRecipeRepository
    {
        private readonly AppDbContext _dbContext;
        public ProductRecipeRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Truyền productId vào để lấy ra danh sách nguyên liệu
        public async Task<List<ProductRecipe>> GetRecipesByProductIdAsync(int productId)
        {
            return await _dbContext.ProductRecipes
                // Lôi thêm thông tin nguyên liệu gốc (Tên, Đơn vị tính) nếu cần show ra
                .Include(pr => pr.InventoryItem) 
                .Where(pr => pr.ProductId == productId)
                .ToListAsync();
        }
    }
}