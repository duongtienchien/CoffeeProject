using CoffeeShop.Models.Entities.Inventory;
using CoffeeShop.Models.Entities.System;
using CoffeeShop.Models.Entities.Auth;
using CoffeeShop.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShop.DAL.Repositories
{
    public class ManagerRepository
    {
        private readonly AppDbContext _dbContext;
        public ManagerRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<StoreInventory> GetInventoryByIdAsync(int itemId)
        {
            return await _dbContext.StoreInventories.FirstOrDefaultAsync(s => s.ItemId == itemId);
        }

        // Ghi sổ cái Giao dịch
        public async Task AddInventoryTransactionAsync(InventoryTransaction transaction)
        {
            await _dbContext.InventoryTransactions.AddAsync(transaction);
        }

        // Ghi Log nếu cần
        public async Task AddSystemLogAsync(SystemAuditLog log)
        {
            await _dbContext.SystemAuditLogs.AddAsync(log);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }
        public async Task<StoreInventory> GetStoreInventoryAsync(int itemId)
        {
            return await _dbContext.StoreInventories.FirstOrDefaultAsync(i => i.ItemId == itemId);
        }

        public async Task AddTransactionAsync(InventoryTransaction transaction)
        {
            await _dbContext.InventoryTransactions.AddAsync(transaction);
        }
        public async Task<List<InventoryTransaction>> GetReceiptTransactionsAsync()
        {
            return await _dbContext.InventoryTransactions
                .Include(t => t.InventoryItem) // Nhớ Include bảng sản phẩm để lấy tên
                .Where(t => t.TransactionType == "Receipt")
                .OrderByDescending(t => t.CreateDate)
                .ToListAsync();
        }
        public async Task<List<SystemAuditLog>> GetCashAlertLogsAsync(DateTime fromDate)
        {
            return await _dbContext.SystemAuditLogs
                .Where(log => (log.Action == "CASH_SHORTAGE" || log.Action == "CASH_SURPLUS" || log.Action == "CASH_DISCREPANCY")
                           && log.CreateDate >= fromDate)
                .OrderByDescending(log => log.CreateDate)
                .ToListAsync();
        }
        // Lấy danh sách tất cả nhân viên
        public async Task<List<User>> GetAllStaffsAsync(int storeId)
        {
            return await _dbContext.Users
            .Include(u => u.UserProfile)
                .Where(u => u.StoreId == storeId && u.Role == "Staff")
                .ToListAsync();
        }

        // Lấy báo cáo chốt ca mới nhất trong ngày của một nhân viên cụ thể
        public async Task<ShiftReport> GetLatestShiftReportAsync(int staffId)
        {
            return await _dbContext.ShiftReports
                .Where(s => s.StaffId == staffId)
                .OrderByDescending(s => s.ShiftEndTime)
                .FirstOrDefaultAsync();
        }
        public async Task<List<SystemAuditLog>> GetSystemWarningsByStoreAsync(int storeId)
{
    // 1. Lấy ra danh sách ID của TẤT CẢ nhân viên thuộc chi nhánh này
    // (Đệ lưu ý thay _dbContext.Users bằng bảng chứa StoreId của đệ, có thể là Staffs hoặc Users)
    var staffIdsInStore = await _dbContext.Users
        .Where(u => u.StoreId == storeId)
        .Select(u => u.Id)
        .ToListAsync();

    // 2. Lọc bảng Log: Chỉ lấy những Log mà UserId nằm trong danh sách nhân viên chi nhánh
    var logs = await _dbContext.SystemAuditLogs
        .Where(log => staffIdsInStore.Contains(log.UserId)) // BÍ KÍP Ở ĐÂY NÀY ĐỆ!
        .OrderByDescending(log => log.CreateDate)
        // Lấy tạm 50 dòng mới nhất cho nhẹ server
        .Take(50) 
        .ToListAsync();

    return logs;
}
    }
}