namespace CoffeeShop.BLL.DTOs.Inventory.Requests
{
    public class CreateOrderRequest
    {
        public int CustomerId { get; set; }
        public int StoreId { get; set; }
        public string PaymentMethod { get; set; }

        // Một bill có thể có nhiều món chính (nhiều dòng order)
        public List<OrderItemRequest> Items { get; set; } = new List<OrderItemRequest>();
    }

    public class OrderItemRequest
    {
        // Ly nước chính
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        
        // Danh sách topping của RIÊNG ly nước này
        public List<ToppingRequestDTO> Toppings { get; set; } = new List<ToppingRequestDTO>();
    }

    public class ToppingRequestDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}