namespace CoffeeShop.BLL.DTOs.Inventory.Requests
{
public class ConfirmPaymentRequest
{
    public Guid OrderId { get; set; }
    public string PaymentMethod { get; set; } 
}
}
