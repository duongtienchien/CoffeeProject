using CoffeeShop.DAL.Data;
using CoffeeShop.Models.Entities.System;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShop.DAL.Repositories
{
    public class ShiftRepository
    {
        private readonly AppDbContext _dbContext;

        public ShiftRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Hàm tính tổng tiền mặt Staff đã bán trong khoảng thời gian
        public async Task<decimal> GetTotalCashInShiftAsync(int staffId, int storeId, DateTime startTime)
        {
            return await _dbContext.Orders
                .Where(o => o.StaffId == staffId 
                         && o.StoreId == storeId 
                         && o.PaymentMethod == "Cash" 
                         && o.CreateDate >= startTime
                         && o.Status == 1) // Ép kiểu OrderStatus.Completed nếu dùng Enum
                .SumAsync(o => o.TotalAmount);
        }

        // Hàm ghi biên bản chốt ca
        public async Task AddShiftReportAsync(ShiftReport report)
        {
            await _dbContext.ShiftReports.AddAsync(report);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}