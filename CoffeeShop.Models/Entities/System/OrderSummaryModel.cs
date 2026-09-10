namespace CoffeeShop.Models.Entities.System
{
    public class OrderSummaryModel
    {
        public Guid OrderId { get; set; }
        public DateTime CreateDate { get; set; }
        public string StaffName { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public int Status { get; set; } // Giữ nguyên số nguyên (1, 2, 3) để BLL tự xử
    }
}