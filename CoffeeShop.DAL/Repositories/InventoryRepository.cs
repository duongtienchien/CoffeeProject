using CoffeeShop.DAL.Data;
using Microsoft.EntityFrameworkCore;
using CoffeeShop.Models.Entities.Inventory;
using CoffeeShop.Models.Entities.Catalog;
using CoffeeShop.Models.Entities.Auth;

namespace CoffeeShop.DAL.Repositories
{
    public class InventoryRepository
    {
        private readonly AppDbContext _dbContext;
        public InventoryRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Đã thêm tham số storeId và đổi tên biến cho chuẩn với CSDL
        public async Task<int> DeductStockForOrderAsync(int storeId, int itemId, decimal quantity)
        {
            return await _dbContext.StoreInventories
                // Phải check CẢ Cửa hàng, CẢ Món ăn, và Số lượng tồn
                .Where(i => i.StoreId == storeId
                         && i.ItemId == itemId
                         && i.Quantity >= quantity)
                .ExecuteUpdateAsync(s => s.SetProperty(i => i.Quantity, i => i.Quantity - quantity));
        }
        public async Task<StoreInventory> GetInventoryAsync(int storeId, int itemId)
        {
            // Phải soi bằng 2 chìa khóa
            return await _dbContext.StoreInventories
                                   .FirstOrDefaultAsync(s => s.StoreId == storeId && s.ItemId == itemId);
        }
        public async Task<InventoryItem> GetInventoryItemByIdAsync(int id)
        {
            return await _dbContext.InventoryItems.FirstOrDefaultAsync(i => i.Id == id);
        }
        // Hàm thêm ghi chú vào sổ cái
        public async Task AddTransactionAsync(InventoryTransaction transaction)
        {
            await _dbContext.InventoryTransactions.AddAsync(transaction);
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        }
        public async Task<List<InventoryTransaction>> GetTransactionHistoryAsync()
        {
            return await _dbContext.InventoryTransactions
                .OrderByDescending(t => t.CreateDate)
                .ToListAsync();
        }
        // 2. Bổ sung hàm Lưu thay đổi xuống Database
        public async Task<bool> SaveChangesAsync()
        {
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }
        public async Task<List<StoreInventory>> GetStoreInventoryAsync(int storeId)
        {
            return await _dbContext.StoreInventories
                // JOIN sang bảng nguyên vật liệu để lấy Tên và Đơn vị tính
                .Include(si => si.InventoryItem)
                // Lọc đúng chi nhánh cần xem
                .Where(si => si.StoreId == storeId)
                .ToListAsync();
        }
        public async Task<List<InventoryTransaction>> GetTransactionHistoryAsync(int storeId) 
{
    return await _dbContext.InventoryTransactions
        .Where(t => t.StoreId == storeId) 
        .OrderByDescending(t => t.CreateDate)
        .ToListAsync();
}
    }
}