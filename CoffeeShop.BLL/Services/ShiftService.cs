using CoffeeShop.DAL.Repositories;
using CoffeeShop.Models.Entities.System;
using CoffeeShop.BLL.DTOs.Inventory.Requests;
using CoffeeShop.DAL.Interfaces;

namespace CoffeeShop.BLL.Services
{
    public class ShiftService
    {
        private readonly ShiftRepository _shiftRepo;
        private readonly ISystemAuditLogRepository _auditLogRepo;

        public ShiftService(ShiftRepository shiftRepo, ISystemAuditLogRepository auditLogRepo)
        {
            _shiftRepo = shiftRepo;
            _auditLogRepo = auditLogRepo;
        }

        public async Task<ShiftReport> CloseShiftAsync(int staffId, int storeId, string staffName, CloseShiftRequest request)
        {
            // Chuyển UTC sang giờ VN, chốt về 0h, rồi trả ngược về UTC để query Database
            var startOfDayUtc = DateTime.UtcNow.AddHours(7).Date.AddHours(-7);

            // 2. Nhờ máy tính đếm tổng tiền từ 0h sáng đến bây giờ
            var systemCash = await _shiftRepo.GetTotalCashInShiftAsync(staffId, storeId, startOfDayUtc);

            // 3. Tính toán độ lệch
            decimal difference = request.ActualCashAmount - systemCash;

            // 4. Lập biên bản 
            var report = new ShiftReport
            {
                StoreId = storeId,
                StaffId = staffId,
                SystemCashAmount = systemCash,
                ActualCashAmount = request.ActualCashAmount,
                Difference = difference,
                Note = request.Note,
                ShiftEndTime = DateTime.UtcNow
            };
            await _shiftRepo.AddShiftReportAsync(report);

            decimal tolerance = 10000m; // Cho phép sai số 10k tiền lẻ

            if (Math.Abs(difference) > tolerance)
            {
                // Nếu chui vào đây nghĩa là CÓ VẤN ĐỀ
                string alertType = difference < 0 ? "THIẾU TIỀN (BIỂN THỦ)" : "DƯ TIỀN (NHẦM LẪN)";

                string description = $"[CẢNH BÁO {alertType}] Staff {staffName} chốt ca lệch {Math.Abs(difference):N0}đ. " +
                                     $"(Hệ thống đếm: {systemCash:N0}đ | Staff nộp: {request.ActualCashAmount:N0}đ). Lý do: {request.Note}";

                await _auditLogRepo.LogActionAsync(new SystemAuditLog
                {
                    Action = difference < 0 ? "CASH_SHORTAGE" : "CASH_SURPLUS",
                    Description = description,
                    UserId = staffId
                });
            }
            // NẾU difference = 0 -> Bỏ qua lệnh if này, không có cảnh báo nào được ghi nhận!

            // 6. Lưu tất cả
            await _shiftRepo.SaveChangesAsync();
            return report;
        }
    }
}