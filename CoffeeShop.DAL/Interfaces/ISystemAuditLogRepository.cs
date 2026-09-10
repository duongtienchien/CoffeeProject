using CoffeeShop.Models.Entities.System;

namespace CoffeeShop.DAL.Interfaces
{
    public interface ISystemAuditLogRepository
    {
        // Ghi lại một hành động mới vào hệ thống
        Task LogActionAsync(SystemAuditLog auditLog);

        // Lấy danh sách log gần nhất để Admin/Manager xem (có giới hạn số lượng để tránh tràn RAM)
        Task<IEnumerable<SystemAuditLog>> GetRecentLogsAsync(int limit = 100);
        Task<List<SystemAuditLog>> GetRecentWarningsAsync(int limit = 3);
        Task<List<SystemAuditLog>> GetSystemWarningsByStoreAsync(int storeId);
    }
}