using Microsoft.EntityFrameworkCore;
using CoffeeShop.Models.Entities.System;
using CoffeeShop.DAL.Interfaces;
using CoffeeShop.DAL.Data;

namespace CoffeeShop.DAL.Repositories
{
    public class SystemAuditLogRepository : ISystemAuditLogRepository
    {
        private readonly AppDbContext _context;

        // Tiêm DbContext vào qua Constructor
        public SystemAuditLogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task LogActionAsync(SystemAuditLog auditLog)
        {
            // Tận dụng async/await để không block thread
            await _context.SystemAuditLogs.AddAsync(auditLog);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<SystemAuditLog>> GetRecentLogsAsync(int limit = 100)
        {
            // Sắp xếp theo thời gian mới nhất đưa lên đầu
            // Dùng AsNoTracking() vì ta chỉ đọc log ra để xem, không có nhu cầu tracking để update
            return await _context.SystemAuditLogs
                                 .AsNoTracking()
                                 .OrderByDescending(log => log.CreateDate)
                                 .Take(limit)
                                 .ToListAsync();
        }
        public async Task<List<SystemAuditLog>> GetRecentWarningsAsync(int limit = 3)
        {
            return await _context.SystemAuditLogs
                .Where(log => log.Description.Contains("CẢNH BÁO"))
                .OrderByDescending(log => log.Id)
                .Take(limit)
                .ToListAsync();
        }
        public async Task<List<SystemAuditLog>> GetSystemWarningsByStoreAsync(int storeId)
{
    // Tìm danh sách ID nhân viên thuộc chi nhánh
    var staffIdsInStore = await _context.Users
        .Where(u => u.StoreId == storeId)
        .Select(u => u.Id)
        .ToListAsync();

    // Lọc chéo Log dựa trên danh sách ID vừa tìm được
    var logs = await _context.SystemAuditLogs
        .Where(log => staffIdsInStore.Contains(log.UserId))
        .OrderByDescending(log => log.Id) // Hoặc CreateDate nếu đệ có cột này
        .Take(50) 
        .ToListAsync();

    return logs;
}
    }
}