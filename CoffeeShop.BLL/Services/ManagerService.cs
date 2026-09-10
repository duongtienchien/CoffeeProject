using CoffeeShop.DAL.Repositories;
using CoffeeShop.BLL.DTOs.Inventory.Requests;
using CoffeeShop.BLL.DTOs.System.Responses;
using CoffeeShop.Models.Entities.Inventory;
using CoffeeShop.Models.Entities.System;

namespace CoffeeShop.BLL.Services
{
    public class ManagerService
    {
        private readonly ManagerRepository _managerRepo;

        public ManagerService(ManagerRepository managerRepo)
        {
            _managerRepo = managerRepo;
        }

        public async Task<bool> AdjustInventoryAsync(int managerId, string managerName, AdjustInventoryRequest request)
        {
            if (request.QuantityChange == 0)
                throw new Exception("Lỗi: Số lượng điều chỉnh phải khác 0!");

            if (string.IsNullOrWhiteSpace(request.Reason))
                throw new Exception("Lỗi: Manager bắt buộc phải nhập lý do điều chỉnh kho!");

            // 1. Móc kho hiện tại lên
            var inventory = await _managerRepo.GetInventoryByIdAsync(request.ItemId);
            if (inventory == null)
                throw new Exception("Lỗi: Không tìm thấy sản phẩm trong kho.");

            // Chặn trường hợp trừ lố kho về số âm
            if (inventory.Quantity + request.QuantityChange < 0)
                throw new Exception($"Lỗi: Trong kho chỉ còn {inventory.Quantity}, không thể trừ đi {Math.Abs(request.QuantityChange)}!");

            // 2. Thực hiện ĐIỀU CHỈNH (+/-)
            inventory.Quantity += request.QuantityChange;

            // 3. GHI SỔ CÁI BẤT BIẾN 
            var transaction = new InventoryTransaction
            {
                StoreId = inventory.StoreId,
                ItemId = request.ItemId,
                QuantityChange = request.QuantityChange,
                TransactionType = "Adjustment", // Đánh dấu đây là lệnh điều chỉnh của Sếp
                Reason = request.Reason,        // Ghi lý do: "Trừ hao hụt do Staff..."
                UnitPrice = 0,                  // Điều chỉnh thì không liên quan giá tiền
                TotalValue = 0,
                CreateBy = managerId,
                CreateDate = DateTime.UtcNow
            };
            await _managerRepo.AddInventoryTransactionAsync(transaction);

            // 4. (Tùy chọn) Bắn Log vàng khè nếu Manager đấm tay trừ một phát quá to (vd: > 50)
            if (request.QuantityChange <= -50)
            {
                await _managerRepo.AddSystemLogAsync(new SystemAuditLog
                {
                    Action = "LARGE_INVENTORY_ADJUSTMENT",
                    Description = $"[CẢNH BÁO HAO HỤT LỚN] Manager {managerName} vừa trừ {Math.Abs(request.QuantityChange)} đơn vị của Item {request.ItemId}. Lý do: {request.Reason}",
                    UserId = managerId
                });
            }

            // 5. Chốt hạ lưu DB
            return await _managerRepo.SaveChangesAsync();
        }
        public async Task<List<object>> GetReceiptsForAuditAsync()
        {
            var receipts = await _managerRepo.GetReceiptTransactionsAsync();

            // Map data ra cho Frontend dễ vẽ bảng
            return receipts.Select(r => new
            {
                TransactionId = r.Id,
                ItemName = r.InventoryItem?.Name ?? "Unknown",
                QuantityChange = r.QuantityChange,
                TotalValue = r.TotalValue,
                Reason = r.Reason, // Nơi chứa Mã Hóa Đơn giấy
                StaffId = r.CreateBy,
                Time = r.CreateDate.ToString("dd/MM/yyyy HH:mm")
            }).ToList<object>();
        }
        public async Task<bool> AdjustInventoryAsync(int managerId, AdjustInventoryRequest request)
        {
            // 1. Kiểm tra tồn kho hiện tại
            var inventory = await _managerRepo.GetStoreInventoryAsync(request.ItemId);
            if (inventory == null) throw new Exception("Không tìm thấy sản phẩm trong kho!");

            // Chặn không cho trừ lố quá số lượng đang có
            if (inventory.Quantity + request.QuantityChange < 0)
                throw new Exception($"Trong kho chỉ còn {inventory.Quantity}, không thể trừ đi {Math.Abs(request.QuantityChange)}!");

            // 2. Cập nhật tồn kho (Cộng số âm = Trừ)
            inventory.Quantity += request.QuantityChange;

            // 3. GHI SỔ CÁI BẤT BIẾN 
            var tx = new InventoryTransaction
            {
                StoreId = inventory.StoreId,
                ItemId = request.ItemId,
                QuantityChange = request.QuantityChange, // Lưu thẳng số -20 vào đây
                TransactionType = "Adjustment", // Đánh dấu là lệnh phạt/điều chỉnh
                Reason = request.Reason,
                UnitPrice = 0,
                TotalValue = 0,
                CreateBy = managerId,
                CreateDate = DateTime.UtcNow
            };

            await _managerRepo.AddTransactionAsync(tx);

            // 4. Lưu cất mẻ lưới
            return await _managerRepo.SaveChangesAsync();
        }
        public async Task<List<CashAlertResponse>> GetTodayCashAlertsAsync()
        {
            // 1. Logic lấy 0h sáng hôm nay theo giờ Việt Nam (UTC+7)
            // Lấy giờ UTC hiện tại
            var nowUtc = DateTime.UtcNow;

            // Chuyển sang giờ Việt Nam (UTC+7)
            var vietnamTime = nowUtc.AddHours(7);

            // Lấy mốc 0h00 sáng ngày HÔM NAY của giờ VN, sau đó lùi lại 7 tiếng để về chuẩn UTC
            var startOfDayUtc = vietnamTime.Date.AddHours(-7);
            // 2. Nhờ Repository móc data dưới DB lên
            var logs = await _managerRepo.GetCashAlertLogsAsync(startOfDayUtc);

            // 3. Map từ Entity sang DTO để trả về cho Controller
            return logs.Select(log => new CashAlertResponse
            {
                Id = log.Id,
                Type = log.Action, // Trả về "CASH_SHORTAGE" để Frontend bắt if-else đổi màu
                Message = log.Description,
                Time = log.CreateDate.ToString("HH:mm")
            }).ToList();
        }
        public async Task<List<ProfileDto>> GetAllStaffsWithShiftDataAsync(int storeId)
        {
            var startOfDayUtc = DateTime.UtcNow.AddHours(7).Date.AddHours(-7);
            var staffs = await _managerRepo.GetAllStaffsAsync(storeId);
            var responseList = new List<ProfileDto>();

            foreach (var staff in staffs)
            {
                var latestShift = await _managerRepo.GetLatestShiftReportAsync(staff.Id);

                // Lưu ý: Nếu user này chưa từng tạo Profile trong DB, nó có thể bị Null.
                // Dùng toán tử ? (Null-conditional) để tránh sập server.
                var profile = staff.UserProfile;

                responseList.Add(new ProfileDto
                {
                    UserId = staff.Id,
                    Username = staff.Email,

                    // Lấy từ bảng UserProfile (Bắt null cẩn thận)
                    FullName = profile?.FullName ?? "Chưa cập nhật",
                    Phone = profile?.Phone ?? "N/A",
                    Avatar = profile?.Avatar ?? "staff.png",

                    // Lấy từ bảng ShiftReport
                    SystemCashAmount = latestShift?.SystemCashAmount ?? 0m,
                    ActualCashAmount = latestShift?.ActualCashAmount ?? 0m,
                    Difference = latestShift?.Difference ?? 0m
                });
            }

            return responseList;
        }
    }
}