namespace CoffeeShop.BLL.DTOs.Inventory.Responses
{
    public class OrderSummaryResponse
    {
        public Guid OrderId { get; set; }
        public DateTime CreateDate { get; set; }
        public string StaffName { get; set; }
        
        // Cột quan trọng: Chỉ lưu con số tổng, tuyệt đối không chứa mảng chi tiết
        public int TotalItems { get; set; } 
        
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string StatusName { get; set; }

        public bool IsFraudWarning { get; set; }
    }
}