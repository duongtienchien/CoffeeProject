using Microsoft.AspNetCore.Mvc;
using CoffeeShop.BLL.DTOs.Inventory.Requests;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CoffeeShop.API.OrderController
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpPost("order")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> CustomerOrder([FromBody] CreateOrderRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "Bạn vẫn chưa Order ?" });
            }
            try
            {
                // Hứng Token do Auth nhả ra để đảm bảo tính nhất quán
                var staffIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                // 2. Chuyển thành số và kiểm tra. 
                if (!int.TryParse(staffIdClaim, out int staffId))
                {
                    return Unauthorized(new { message = "Token không chứa ID nhân viên hợp lệ hoặc không có quyền!" });
                }

                // Dùng ClaimTypes.Name, nếu không có thì fallback về "Nhân viên Vô Danh" để bill không bị null
                var staffName = User.FindFirst(ClaimTypes.Name)?.Value ?? "Nhân viên Vô Danh";

                // 2. Ném hộp xuống tầng BLL (OrderService) để nó xử lý DB và tính tiền.
                // Hứng lại cái hóa đơn từ BLL trả lên.
                var responseDto = await _orderService.CreateNewOrderAsync(request, staffId, staffName);
                // 3. Trả về mã 201 kèm cái hóa đơn cho Frontend in ra bill
                return StatusCode(201, new
                {
                    message = "Ok rồi nhé, bill của bạn đây",
                    data = responseDto
                });
            }
            catch (Exception ex)
            {
                // Nếu tầng BLL check DB thấy món nước không tồn tại, nó ném lỗi lên đây
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("confirm-payment")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentRequest request)
        {
            try
            {
                // Lấy thẻ ngành ra tự bóc ID và Tên (Y hệt hàm Order)
                var staffIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(staffIdClaim, out int staffId))
                {
                    return Unauthorized(new { message = "Token không chứa ID nhân viên hợp lệ hoặc không có quyền!" });
                }

                var staffName = User.FindFirst(ClaimTypes.Name)?.Value ?? "Nhân viên Vô Danh";

                // Lễ tân gọi BLL xử lý, truyền cục data vừa bóc được từ Token xuống
                var response = await _orderService.ConfirmPaymentAsync(request, staffId, staffName);

                return Ok(response);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Order_Not_Found")
                {
                    return NotFound(new { error = "Không tìm thấy hoá đơn này!" });
                }
                if (ex.Message == "Order_Already_Paid")
                {
                    return BadRequest(new { error = "Hoá đơn này đã được thanh toán từ trước!" });
                }

                return StatusCode(500, new { error = "Lỗi hệ thống: " + ex.Message });
            }
        }
        [HttpPost("{orderId}/cancel")]
        [Authorize(Roles = "Staff,Manager")] 
        public async Task<IActionResult> CancelOrder(Guid orderId, [FromBody] CancelOrderRequestDto request)
        {
            try
            {
                // 1. Tận dụng bóc Token 
                var staffIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(staffIdClaim, out int staffId))
                {
                    return Unauthorized(new { message = "Token không chứa ID nhân viên hợp lệ hoặc không có quyền!" });
                }

                // 2. Gọi BLL xử lý nghiệp vụ hủy đơn và nhả kho
                bool result = await _orderService.CancelOrderAsync(orderId, staffId, request);

                if (result)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Hủy đơn hàng và hoàn trả nguyên liệu thành công!"
                    });
                }

                return BadRequest(new { success = false, message = "Có lỗi xảy ra, không thể hủy đơn hàng." });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Cảnh báo bảo mật") || ex.Message.Contains("Không tìm thấy"))
                {
                    return BadRequest(new { success = false, message = ex.Message });
                }

                Console.WriteLine($"[LỖI HỆ THỐNG - CancelOrder] {ex.ToString()}");
                return StatusCode(500, new { success = false, message = "Đã có lỗi hệ thống xảy ra, vui lòng thử lại sau!" });
            }
        }
        [HttpGet]
        [Authorize(Roles = "Manager,Staff")]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                // Gọi Service lấy danh sách
                var orders = await _orderService.GetOrderSummariesAsync();

                // Trả về JSON bọc trong biến 'data' (để khớp với code JavaScript result.data)
                return Ok(new { data = orders });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
        [HttpGet("store/{storeId}")] // Đường dẫn sẽ là /api/Order/store/1
        [Authorize(Roles = "Manager, Staff")]
        public async Task<IActionResult> GetOrdersByStore(int storeId)
        {
            // Gọi xuống Service/Repo để lấy đơn hàng where StoreId == storeId
            var orders = await _orderService.GetOrdersByStoreIdAsync(storeId);

            return Ok(new
            {
                message = $"Lấy đơn hàng chi nhánh {storeId} thành công",
                data = orders
            });
        }
    }
}