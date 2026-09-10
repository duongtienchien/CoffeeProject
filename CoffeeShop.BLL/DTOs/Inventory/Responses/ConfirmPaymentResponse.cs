namespace CoffeeShop.BLL.DTOs.Inventory.Requests
{
    public class ConfirmPaymentResponse
{
    public Guid OrderId { get; set; }
    public string StatusName { get; set; } 
    public string Message { get; set; }
}
}
