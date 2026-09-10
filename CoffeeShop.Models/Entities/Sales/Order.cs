namespace CoffeeShop.Models.Entities.Sales
{
    public class Order 
    {
        public Guid Id { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public int Status { get; set; }
        public int StaffId { get; set; }
        public int StoreId { get; set; }
        public int CustomerId { get; set; }
        // THÊM DÒNG NÀY ĐỂ EF CORE BIẾT STAFFID LÀ CỦA BẢNG USER
        public virtual Auth.User Staff { get; set; }
        public virtual Store Store { get; set; }
        public virtual Customer Customer { get; set; }
        public string? CancellationReason { get; set; }
public DateTime? CanceledAt { get; set; }
public bool IsFraudWarning { get; set; }
        //Lý do để dấu bằng là để tránh lỗi NULL
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}