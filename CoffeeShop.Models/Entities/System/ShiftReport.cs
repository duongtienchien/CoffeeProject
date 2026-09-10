// File: CoffeeShop.Models/Entities/System/ShiftReport.cs
namespace CoffeeShop.Models.Entities.System
{
    public class ShiftReport
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public int StaffId { get; set; }
        
        // Tiền mặt máy tính tính ra (Cộng dồn các Order trả bằng Cash)
        public decimal SystemCashAmount { get; set; } 
        
        // Tiền mặt Staff tự đếm và gõ vào
        public decimal ActualCashAmount { get; set; } 
        
        // Tiền chênh lệch (Actual - System)
        public decimal Difference { get; set; } 
        
        public string Note { get; set; } // Ghi chú (VD: "Thiếu 5k do không có tiền lẻ thối")
        public DateTime ShiftEndTime { get; set; } = DateTime.UtcNow;
    }
}
// Nhớ thêm public DbSet<ShiftReport> ShiftReports { get; set; } vào AppDbContext và chạy Migration nhé!