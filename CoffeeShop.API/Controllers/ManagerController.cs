using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CoffeeShop.BLL.Services;
using System.Security.Claims;
using CoffeeShop.BLL.DTOs.Inventory.Requests;

namespace CoffeeShop.API.ManagerController
{
    [ApiController]
    [Route("api/[controller]")]
    public class ManagerController : ControllerBase
    {
        private readonly InventoryService _inventoryService;
        private readonly UserProfileService _userprofileService;

        private readonly SystemAuditLogService _auditLogService;

        private readonly ManagerService _managerService;


        // DI tiêm Service vào Controller
        public ManagerController(InventoryService inventoryService, 
        UserProfileService userprofileService, 
        SystemAuditLogService auditLogService, 
        ManagerService managerService)
        {
            _inventoryService = inventoryService;
            _userprofileService = userprofileService;
            _auditLogService = auditLogService;
            _managerService = managerService;
        }

        [Authorize(Roles = "Manager")]
        [HttpGet("manager-data")]
        public IActionResult Manager()
        {
            var data = new { message = "Manager controller" };
            return Ok(data);
        }

        [Authorize(Roles = "Manager")]
        [HttpGet("inventory-transactions/{storeId}")]
        public async Task<IActionResult> GetInventoryTransactions()
        {

            try
            {
                var data = await _inventoryService.GetHistoryAsync();

                return Ok(new
                {
                    message = "Lấy lịch sử giao dịch thành công!",
                    data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpGet("store-inventory/{storeId}")]
        public async Task<IActionResult> GetInventoryByStore(int storeId)
        {
            if (storeId <= 0)
            {
                return BadRequest(new { message = "Mã chi nhánh không hợp lệ!" });
            }

            try
            {
                var data = await _inventoryService.GetInventoryByStoreIdAsync(storeId);

                return Ok(new
                {
                    message = $"Lấy tồn kho của chi nhánh {storeId} thành công!",
                    data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("staffs/{storeId}")]
        public async Task<IActionResult> GetStaffs(int storeId)
        {
            try
            {
                var staffList = await _managerService.GetAllStaffsWithShiftDataAsync(storeId);
                return Ok(new { data = staffList });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi Server: " + ex.Message });
            }
        }
        [HttpGet("me")]
        [Authorize] // Bắt buộc có vé
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value
                         ?? User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(email))
                    return Unauthorized(new { message = "Token không hợp lệ hoặc thiếu Email" });

                var myInfo = await _userprofileService.GetMyProfileAsync(email);

                if (myInfo == null)
                    return NotFound(new { message = "Không tìm thấy người dùng" });

                //Trả hàng về cho Frontend
                return Ok(new { data = myInfo });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi Server: " + ex.Message });
            }
        }
        [HttpGet("system-warnings/{storeId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetSystemWarnings(int storeId) //Hứng tham số storeId
        {
            if (storeId <= 0)
            {
                return BadRequest(new { message = "Mã chi nhánh không hợp lệ!" });
            }

            try
            {
                //Truyền storeId xuống tầng Service (BLL)
                var warnings = await _auditLogService.GetSystemWarningsByStoreAsync(storeId);
                
                return Ok(new { data = warnings });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
        [HttpPost("adjust-inventory")]
        [Authorize(Roles = "Manager")] 
        public async Task<IActionResult> AdjustInventory([FromBody] AdjustInventoryRequest request)
        {
            try
            {
                // Bóc ID và Tên của Manager từ Token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userNameClaim = User.FindFirst(ClaimTypes.Name)?.Value ?? "Quản Lý Không Tồn Tại";

                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized(new { message = "Token không hợp lệ!" });

                int managerId = int.Parse(userIdClaim);

                // Ném xuống cho Service xử lý
                await _managerService.AdjustInventoryAsync(managerId, userNameClaim, request);

                return Ok(new
                {
                    success = true,
                    message = "Điều chỉnh kho thành công! Đã ghi nhận vào sổ."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        [HttpGet("receipts-audit")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetReceiptsForAudit()
        {
            try
            {
                var data = await _managerService.GetReceiptsForAuditAsync();
                return Ok(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        [HttpGet("cash-alerts")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetCashAlerts()
        {
            try
            {
                var alerts = await _managerService.GetTodayCashAlertsAsync();
                return Ok(new { success = true, data = alerts });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}