using Microsoft.EntityFrameworkCore;
using CoffeeShop.Models.Entities.Sales;
using CoffeeShop.DAL.Data;
using CoffeeShop.Models.Entities.System;
using CoffeeShop.Models.Entities.Enums;
using Microsoft.EntityFrameworkCore.Storage;
namespace CoffeeShop.DAL.Repositories
{
    public class OrderRepository
    {
        private readonly AppDbContext _dbContext;
        public OrderRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<OrderDetail>> GetOrderDetailsByOrderIdAsync(Guid orderId)
        {
            return await _dbContext.OrderDetails.Where(o => o.OrderId == orderId).ToListAsync();
        }
        public async Task<Order> GetOrderByIdAsync(Guid orderId)
        {
            return await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
        }
        public async Task<bool> SaveOrderAsync(Order order, List<OrderDetail> orderDetail)
        {
            //Buoc 1: Nem hoa don vao gio cho
            await _dbContext.Orders.AddAsync(order);
            //Buoc 2: Nem chi tiet vao gio cho
            await _dbContext.OrderDetails.AddRangeAsync(orderDetail);
            //Buoc 3: Chot
            var result = await _dbContext.SaveChangesAsync();
            //Neu result lon hon 0 => Thanh Cong
            return result > 0;
        }
        public async Task<bool> UpdateOrderStatusAsync(Guid Id, string paymentMethod)
        {
            // Lấy đúng cái Order cần update lên
            var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == Id);

            // Nếu không tìm thấy đơn, báo lỗi (false)
            if (order == null) return false;

            // Chỉ cập nhật đúng 2 trường cần thiết, tuyệt đối không đụng tới OrderDetails
            order.Status = (int)OrderStatus.Completed;
            order.PaymentMethod = paymentMethod;

            _dbContext.Orders.Update(order);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }
        //Hàm chuyên dụng cho việc huỷ đơn
        public async Task<bool> UpdateOrderAsync(Order order)
        {
            _dbContext.Orders.Update(order);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }
        public Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return _dbContext.Database.BeginTransactionAsync();
        }
        public async Task<List<OrderSummaryModel>> GetRawOrderSummariesAsync()
        {
            // Bắn DUY NHẤT 1 câu lệnh SQL phức tạp xuống DB
            return await _dbContext.Orders
                .Select(o => new OrderSummaryModel
                {
                    OrderId = o.Id,
                    CreateDate = o.CreateDate,
                    StaffName = _dbContext.UserProfiles.Where(u => u.UserId == o.StaffId).Select(u => u.FullName).FirstOrDefault() ?? "Vô Danh",
                    TotalItems = _dbContext.OrderDetails.Count(od => od.OrderId == o.Id),
                    TotalAmount = o.TotalAmount,
                    PaymentMethod = o.PaymentMethod,
                    Status = o.Status // Trả về số thô
                })
                .OrderByDescending(o => o.CreateDate)
                .ToListAsync();
        }
        public async Task<List<Order>> GetOrdersByStoreIdAsync(int storeId)
        {
            return await _dbContext.Orders
                .Where(o => o.StoreId == storeId) // Móc đúng đơn của chi nhánh
                                                  // .Include(o => o.OrderDetails) // Mở comment dòng này ra nếu đệ cần lấy thêm số lượng món (TotalItems)
                .Include(o => o.Staff)        // Mở ra nếu đệ muốn lấy Tên nhân viên thật từ bảng Staff
                .OrderByDescending(o => o.CreateDate)
                .ToListAsync();
        }
        // Thêm hàm này vào bên trong class OrderRepository
        public async Task<int> CountRecentCanceledOrdersAsync(int staffId, DateTime since)
{
    // Quét thẳng vào Camera An Ninh (SystemAuditLogs) thay vì quét hóa đơn
    return await _dbContext.SystemAuditLogs
        .Where(log => log.UserId == staffId 
                   // Phải đếm cả 2 loại log này vì khi chạm mốc nó đổi tên Action
                   && (log.Action == "CANCEL_ORDER" || log.Action == "FRAUD_WARNING") 
                   && log.CreateDate >= since)
        .CountAsync();
}
    }
}