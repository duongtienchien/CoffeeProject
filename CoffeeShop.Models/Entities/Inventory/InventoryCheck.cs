namespace CoffeeShop.Models.Entities.Inventory
{
    public class InventoryCheck
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // 1. Tọa độ kiểm tra
        public int StoreId { get; set; } // Nhớ đổi thành Guid nếu StoreId của đệ là Guid
        public int ItemId { get; set; }  // Nhớ đổi thành Guid nếu ItemId của đệ là Guid

        // 2. Bằng chứng đối soát
        public decimal SystemQuantity { get; set; } // Số lượng sổ sách (Máy tính báo)

        public decimal ActualQuantity { get; set; } // Số lượng thực tế (Quản lý cân được)

        public decimal Discrepancy { get; set; } // Độ lệch = Actual - System (Âm là mất hàng, Dương là dư hàng)

        // 3. Dấu vết người dùng
        public int ManagerId { get; set; } // Mã quản lý đi kiểm kho
        public DateTime CheckDate { get; set; } = DateTime.UtcNow; // Thời gian kiểm tra

        // 4. Luồng nghiệp vụ
        public int Status { get; set; } // Trạng thái phiếu (Dùng Enum ở Bước 2)
        public int? ResolvedTransactionId { get; set; }

        public string? Explanation { get; set; } // Lời khai của nhân viên (Cho phép null lúc mới tạo)
        // Dùng để EF Core tự động Join các bảng
        public virtual Store Store { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }
        public virtual Auth.User Manager { get; set; }
        public virtual InventoryTransaction ResolvedTransaction { get; set; }
    }
}