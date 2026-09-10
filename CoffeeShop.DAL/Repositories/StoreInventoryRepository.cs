using Microsoft.EntityFrameworkCore;
using CoffeeShop.Models.Entities.Inventory;
using CoffeeShop.DAL.Data;

namespace CoffeeShop.DAL.Repositories
{
    public class StoreInventoryRepository
    {
        private readonly AppDbContext _dbContext;

        public StoreInventoryRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // 1. Lấy thông tin tồn kho của 1 nguyên liệu tại 1 cửa hàng cụ thể
        public async Task<StoreInventory> GetInventoryAsync(int storeId, int itemId)
        {
            return await _dbContext.StoreInventories
                .FirstOrDefaultAsync(si => si.StoreId == storeId && si.ItemId == itemId);
        }

        // 2. Đánh dấu bản ghi này cần được cập nhật
        public void UpdateInventory(StoreInventory inventory)
        {
            _dbContext.StoreInventories.Update(inventory);
        }
    }
}