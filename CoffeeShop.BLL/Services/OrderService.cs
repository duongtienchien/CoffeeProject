using CoffeeShop.BLL.DTOs.Inventory.Requests;
using CoffeeShop.BLL.DTOs.Inventory.Responses;
using CoffeeShop.Models.Entities.Sales;
using CoffeeShop.DAL.Repositories;
using CoffeeShop.Models.Entities.Enums;
using CoffeeShop.Models.Entities.System;
using CoffeeShop.DAL.Interfaces;

public class OrderService
{
    private readonly ProductRepository _productRepo;
    private readonly OrderRepository _orderRepo;

    private readonly ProductRecipeRepository _productrecipeRepo;
    private readonly StoreInventoryRepository _storeinventoryRepo;
    private readonly ISystemAuditLogRepository _systemauditlogRepo;
    private readonly InventoryRepository _inventoryRepo;

    public OrderService(ProductRepository productRepo,
    OrderRepository orderRepo,
    ProductRecipeRepository productrecipeRepo,
    StoreInventoryRepository storeinventoryRepo,
    ISystemAuditLogRepository systemauditlogRepo,
    InventoryRepository inventoryRepo
    )
    {
        _productRepo = productRepo;
        _orderRepo = orderRepo;
        _productrecipeRepo = productrecipeRepo;
        _storeinventoryRepo = storeinventoryRepo;
        _systemauditlogRepo = systemauditlogRepo;
        _inventoryRepo = inventoryRepo;
    }

    // Nhận thêm staffId từ tầng Controller (nơi đã giải mã JWT Token)
    public async Task<OrderResponse> CreateNewOrderAsync(CreateOrderRequest request, int staffId, string staffName)
    {
        //Mở Transaction để đảm bảo tính toàn vẹn dữ liệu (Rollback nếu lỗi)
        using var transaction = await _orderRepo.BeginTransactionAsync();
        // 1. Khởi tạo Order 
        var newOrder = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            StoreId = request.StoreId,
            StaffId = staffId, // Lưu vết người tạo đơn!
            CreateDate = DateTime.UtcNow,
            Status = 1, // 1 là Pending
            PaymentMethod = request.PaymentMethod,
            TotalAmount = 0
        };

        var orderDetailsList = new List<OrderDetail>();
        var responseItems = new List<OrderItemResponse>();
        var storeId = request.StoreId;
        foreach (var itemReq in request.Items)
        {
            var recipes = await _productrecipeRepo.GetRecipesByProductIdAsync(itemReq.ProductId);
            if (recipes == null || !recipes.Any())
            {
                throw new Exception($"Quán chưa có công thức cho món ID: {itemReq.ProductId}");
            }
            // Duyệt qua TỪNG NGUYÊN LIỆU trong công thức để trừ kho
            foreach (var recipeItem in recipes)
            {
                // Tính toán số lượng cần: (Số nguyên liệu cho 1 ly) x (Số ly khách đặt)
                decimal quantityNeeded = recipeItem.QuantityNeeded * itemReq.Quantity;
                // Bắn thẳng lệnh xuống DB, hàm này sẽ trả về số dòng bị ảnh hưởng
                var rowsAffected = await _inventoryRepo.DeductStockForOrderAsync(storeId, recipeItem.ItemId, quantityNeeded);

                // Nếu = 0 tức là: Hoặc chưa nhập kho bao giờ, Hoặc số lượng trong kho < quantityNeeded
                if (rowsAffected == 0)
                {
                    throw new Exception($"Không đủ nguyên liệu ID {recipeItem.ItemId} để pha chế món {itemReq.ProductId}! Giao dịch bị hủy.");
                }

            }
            // XỬ LÝ TRỪ KHO CHO TOPPING
            if (itemReq.Toppings != null && itemReq.Toppings.Any())
            {
                foreach (var toppingReq in itemReq.Toppings)
                {
                    // BƯỚC 1: Lấy thông tin product lên để check
                    var toppingProduct = await _productRepo.GetProductByIdAsync(toppingReq.ProductId);
                    if (toppingProduct == null)
                    {
                        throw new Exception($"Không tìm thấy sản phẩm có ID: {toppingReq.ProductId}");
                    }

                    // BƯỚC 2: RÀO LỖ HỔNG (Kiểm tra xem nó có đích thị là Topping không)
                    // Category Topping của tui có ID là 4 
                    if (toppingProduct.CategoryId != 4)
                    {
                        throw new Exception($"Bắt quả tang gian lận! Sản phẩm '{toppingProduct.Name}' không phải là Topping!");
                    }

                    // BƯỚC 3: Nếu qua ải kiểm tra, tiếp tục trừ kho
                    var toppingRecipes = await _productrecipeRepo.GetRecipesByProductIdAsync(toppingReq.ProductId);

                    if (toppingRecipes == null || !toppingRecipes.Any())
                    {
                        throw new Exception($"Quán chưa có công thức cho Topping: {toppingProduct.Name}");
                    }

                    foreach (var toppingRecipeItem in toppingRecipes)
                    {
                        decimal toppingQtyNeeded = toppingRecipeItem.QuantityNeeded * toppingReq.Quantity;

                        var rowsAffected = await _inventoryRepo.DeductStockForOrderAsync(storeId, toppingRecipeItem.ItemId, toppingQtyNeeded);

                        if (rowsAffected == 0)
                        {
                            throw new Exception($"Không đủ nguyên liệu cho Topping {toppingProduct.Name}! Giao dịch bị hủy.");
                        }
                    }
                }
            }
        }

        // 2. Xử lý từng ly nước trong mảng Items
        foreach (var itemReq in request.Items)
        {
            var productEntity = await _productRepo.GetProductByIdAsync(itemReq.ProductId);
            if (productEntity == null)
            {
                throw new Exception($"Quán không có bán món có ID: {itemReq.ProductId}");
            }

            // Tạo chi tiết cho ly nước CHÍNH
            var mainDetailId = Guid.NewGuid();
            var mainDetail = new OrderDetail
            {
                Id = mainDetailId,
                OrderId = newOrder.Id,
                ProductId = productEntity.Id,
                Quantity = itemReq.Quantity,
                Price = productEntity.Price,
                ParentOrderDetailId = null // Đồ uống chính thì không có Parent
            };
            orderDetailsList.Add(mainDetail);

            decimal mainSubTotal = productEntity.Price * itemReq.Quantity;
            var responseToppings = new List<ToppingResponseDTO>();

            // 3. Xử lý Topping ăn theo ly nước chính này
            if (itemReq.Toppings != null && itemReq.Toppings.Any())
            {
                foreach (var toppingReq in itemReq.Toppings)
                {
                    var toppingEntity = await _productRepo.GetProductByIdAsync(toppingReq.ProductId);
                    if (toppingEntity == null)
                    {
                        throw new Exception($"Không tìm thấy Topping có ID: {toppingReq.ProductId}");
                    }

                    var toppingDetail = new OrderDetail
                    {
                        Id = Guid.NewGuid(),
                        OrderId = newOrder.Id,
                        ProductId = toppingEntity.Id,
                        Quantity = toppingReq.Quantity,
                        Price = toppingEntity.Price,
                        ParentOrderDetailId = mainDetailId // TRÓI VÀO LY NƯỚC CHÍNH BẰNG ID NÀY
                    };
                    orderDetailsList.Add(toppingDetail);

                    // Cộng tiền topping vào tổng tiền của ly này
                    mainSubTotal += (toppingEntity.Price * toppingReq.Quantity);

                    responseToppings.Add(new ToppingResponseDTO
                    {
                        ProductId = toppingEntity.Id,
                        ProductName = toppingEntity.Name,
                        Quantity = toppingReq.Quantity,
                        Price = toppingEntity.Price
                    });
                }
            }

            // Cộng tiền ly này (gồm cả topping) vào tổng tiền của cả Bill
            newOrder.TotalAmount += mainSubTotal;

            // Đóng gói thông tin ly này để trả về
            responseItems.Add(new OrderItemResponse
            {
                ProductId = productEntity.Id,
                ProductName = productEntity.Name,
                Quantity = itemReq.Quantity,
                Price = productEntity.Price,
                SubTotal = mainSubTotal,
                Toppings = responseToppings
            });
        }

        // 4. Lưu xuống Database 
        // Chỗ này cần lưu mảng orderDetailsList thay vì 1 object đơn lẻ
        await _systemauditlogRepo.LogActionAsync(new SystemAuditLog
        {
            Action = "CREATE_ORDER",
            // Ghi chú siêu chi tiết: Ai bán, bán bill nào, thu bao nhiêu tiền
            Description = $"{staffName} (ID: {staffId}) vừa tạo thành công đơn hàng {newOrder.Id} trị giá {newOrder.TotalAmount} VNĐ.",
            UserId = staffId
        });
        await _orderRepo.SaveOrderAsync(newOrder, orderDetailsList);
        await transaction.CommitAsync();

        // 5. Trả về Response
        return new OrderResponse
        {
            OrderId = newOrder.Id,
            CreateDate = newOrder.CreateDate,
            Status = "Pending",
            TotalAmount = newOrder.TotalAmount,
            PaymentMethod = newOrder.PaymentMethod,
            StaffId = staffId,
            StaffName = staffName,
            Items = responseItems
        };
    }
    public async Task<ConfirmPaymentResponse> ConfirmPaymentAsync(ConfirmPaymentRequest request, int staffId, string staffName)
    {
        // 1. Lấy thông tin đơn hàng lên
        var order = await _orderRepo.GetOrderByIdAsync(request.OrderId);

        // 2. RÀO CẢN BẢO VỆ (Phải kiểm tra TRƯỚC KHI sửa dữ liệu)
        if (order == null)
        {
            throw new Exception("Order_Not_Found");
        }

        if (order.Status == (int)OrderStatus.Completed)
        {
            throw new Exception("Order_Already_Paid");
        }

        // 3. CẬP NHẬT TRẠNG THÁI (Sau khi đã qua vòng kiểm tra an toàn)
        order.Status = (int)OrderStatus.Completed;
        order.PaymentMethod = request.PaymentMethod;

        // 4. LƯU XUỐNG DATABASE (Chỉ cần 1 lệnh này thôi, xóa sạch mấy cái biến updateSuccess cũ đi)
        await _orderRepo.UpdateOrderAsync(order);

        // 5. GHI LOG (Đã sửa lại văn phong thanh toán)
        await _systemauditlogRepo.LogActionAsync(new SystemAuditLog
        {
            Action = "CONFIRM_PAYMENT", // Đổi tên Action cho chuẩn
            Description = $"Nhân viên {staffName} (ID: {staffId}) vừa xác nhận thanh toán thành công đơn hàng {order.Id} trị giá {order.TotalAmount} VNĐ.",
            UserId = staffId
        });

        // 6. TRẢ KẾT QUẢ
        return new ConfirmPaymentResponse
        {
            OrderId = order.Id,
            StatusName = "Completed",
            Message = "Thanh toán thành công rực rỡ!"
        };
    }

    public async Task<bool> CancelOrderAsync(Guid orderId, int staffId, CancelOrderRequestDto request)
    {
        // Bước 1: Lấy Order và danh sách món (OrderDetails) của cái bill này lên
        var order = await _orderRepo.GetOrderByIdAsync(orderId);
        if (order == null) throw new Exception("Không tìm thấy đơn hàng!");

        if (order.Status == (int)OrderStatus.Completed)
        {
            // Tung Exception thẳng mặt, ghi log lại thằng staffId nào vừa cố tình ấn Hủy đơn này!
            throw new Exception($"Cảnh báo bảo mật: Đơn hàng {orderId} đã thanh toán, không được phép Hủy!");
        }

        // Nếu đơn đã Hủy từ trước rồi thì thôi, không làm gì cả
        if (order.Status == (int)OrderStatus.Cancelled) return true;

        // NGHIỆP VỤ NHẢ KHO (RELEASE RESERVE STOCK) - Giữ nguyên logic siêu chuẩn của đệ
        var orderDetails = await _orderRepo.GetOrderDetailsByOrderIdAsync(orderId);
        foreach (var detail in orderDetails)
        {
            var recipes = await _productrecipeRepo.GetRecipesByProductIdAsync(detail.ProductId);
            foreach (var recipeItem in recipes)
            {
                decimal qtyToRefund = recipeItem.QuantityNeeded * detail.Quantity;
                var inventory = await _storeinventoryRepo.GetInventoryAsync(order.StoreId, recipeItem.ItemId);
                if (inventory != null)
                {
                    inventory.Quantity += qtyToRefund;
                    _storeinventoryRepo.UpdateInventory(inventory);
                }
            }
        }

        // ĐỔI TRẠNG THÁI VÀ GHI NHẬN THÔNG TIN HỦY
        order.Status = (int)OrderStatus.Cancelled;
        order.CancellationReason = request.CancellationReason; // Bắt lý do từ DTO
        order.CanceledAt = DateTime.UtcNow;

        // Đếm xem 4 tiếng qua đã hủy mấy đơn?
        var checkTime = DateTime.UtcNow.AddHours(-4);
        var previousCanceledCount = await _orderRepo.CountRecentCanceledOrdersAsync(staffId, checkTime);

        // 1. Đánh cờ đỏ (IsFraudWarning) cho TẤT CẢ các đơn từ số 3 trở đi
        order.IsFraudWarning = previousCanceledCount >= 2;
        await _orderRepo.UpdateOrderAsync(order);

        if (previousCanceledCount >= 2)
        {
            await _systemauditlogRepo.LogActionAsync(new SystemAuditLog
            {
                Action = "FRAUD_WARNING",
                Description = $"[CẢNH BÁO GIAN LẬN] Nhân viên (ID: {staffId}) hủy đơn thứ {previousCanceledCount + 1} trong ca. Lý do: {request.CancellationReason}",
                UserId = staffId
            });
        }
        else 
        {
            // Hủy đơn 1 và 2 thì nhắm mắt cho qua, chỉ ghi log thường
            await _systemauditlogRepo.LogActionAsync(new SystemAuditLog
            {
                Action = "CANCEL_ORDER",
                Description = $"Nhân viên (ID: {staffId}) đã hủy đơn. Lý do: {request.CancellationReason}",
                UserId = staffId
            });
        }
        
        return true;
    }
    public async Task<List<OrderSummaryResponse>> GetOrderSummariesAsync()
    {
        // 1. Gọi DAL lấy dữ liệu thô (Không đụng chạm gì tới DbContext)
        var rawModels = await _orderRepo.GetRawOrderSummariesAsync();

        // 2. Chuyển đổi (Mapping) từ Model sang DTO và xử lý Logic nghiệp vụ (Status)
        var dtoList = rawModels.Select(m => new OrderSummaryResponse
        {
            OrderId = m.OrderId,
            CreateDate = m.CreateDate,
            StaffName = m.StaffName,
            TotalItems = m.TotalItems,
            TotalAmount = m.TotalAmount,
            PaymentMethod = m.PaymentMethod,
            // BLL xử lý logic dịch trạng thái từ số sang chữ
            StatusName = m.Status == 1 ? "Pending" : (m.Status == 2 ? "Completed" : "Cancelled")
        }).ToList();

        return dtoList;
    }
    public async Task<List<OrderSummaryResponse>> GetOrdersByStoreIdAsync(int storeId)
    {
        // 1. Chỉ gọi Repo lấy data thô (Sạch sẽ, không dính líu DB)
        var orders = await _orderRepo.GetOrdersByStoreIdAsync(storeId);

        // 2. Map (chế biến) nó sang DTO giống hệt như đệ đã làm ở hàm cũ
        var result = orders.Select(o => new OrderSummaryResponse
        {
            OrderId = o.Id,
            CreateDate = o.CreateDate,
            TotalAmount = o.TotalAmount,
            PaymentMethod = o.PaymentMethod,
            StatusName = o.Status == 1 ? "Pending" : (o.Status == 2 ? "Completed" : "Voided"),
            IsFraudWarning = o.IsFraudWarning,
            StaffName = "Nhân viên " + o.StaffId,
            TotalItems = 2
        }).ToList();
        return result;
    }
}