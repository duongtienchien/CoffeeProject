using CoffeeShop.DAL.Interfaces;
using CoffeeShop.BLL.DTOs.System.Responses;
using CoffeeShop.Models.Entities.System; 

namespace CoffeeShop.BLL.Services
{
    public class SystemAuditLogService
    {
        private readonly ISystemAuditLogRepository _systemAuditLogRepo;

        public SystemAuditLogService(ISystemAuditLogRepository systemAuditLogRepo)
        {
            _systemAuditLogRepo = systemAuditLogRepo;
        }

        public async Task<List<SystemWarningResponseDto>> GetRecentWarningsAsync(int limit = 3)
        {
            var logs = await _systemAuditLogRepo.GetRecentWarningsAsync(limit);

            var dtoList = logs.Select(log => new SystemWarningResponseDto
            {
                Id = log.Id,
                Action = log.Action,
                Description = log.Description
            }).ToList();

            return dtoList;
        }

        public async Task<List<SystemWarningResponseDto>> GetSystemWarningsByStoreAsync(int storeId)
        {
            var logs = await _systemAuditLogRepo.GetSystemWarningsByStoreAsync(storeId); 
            
            return logs.Select(log => new SystemWarningResponseDto
            {
                Id = log.Id,
                Action = log.Action,
                Description = log.Description
            }).ToList();
        }
    }
}