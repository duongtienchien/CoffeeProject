namespace CoffeeShop.BLL.DTOs.Inventory.Responses
{
    public class OrderResponse 
    {
        public Guid OrderId { get; set; }
        public DateTime CreateDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }

        // Bổ sung thông tin để in Bill
        public int StaffId { get; set; }
        public string StaffName { get; set; } // Bill in ra phải có tên nhân viên 
        // 1. Lý do hủy (để Manager đọc xem có vô lý không, VD: "Khách chê dở" nhưng làm gì có khách nào)
        public string? CancellationReason { get; set; } 
        
        // 2. Thời gian hủy (Cực kỳ quan trọng để Manager đối chiếu với giờ trên Camera an ninh)
        public DateTime? CanceledAt { get; set; } 
        
        // 3. Cờ cảnh báo: Trả về true nếu lúc hủy đơn này, hệ thống phát hiện 
        // tên Staff này đã hủy >= 3 đơn trong ca làm việc. UI sẽ bắt cờ này để bôi đỏ dòng Order.
        public bool IsFraudWarning { get; set; }

        public List<OrderItemResponse> Items { get; set; } = new List<OrderItemResponse>();
    }

    public class OrderItemResponse 
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; } // Giá 1 ly
        public decimal SubTotal { get; set; } // Tổng tiền của ly này (đã cộng topping)

        public List<ToppingResponseDTO> Toppings { get; set; } = new List<ToppingResponseDTO>();
    }

    public class ToppingResponseDTO 
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}