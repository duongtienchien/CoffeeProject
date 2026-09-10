namespace CoffeeShop.BLL.DTOs.Inventory.Responses
{
    public class StaffResponse
    {
        public string ItemName { get; set; }
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public decimal NewStockQuantity { get; set; }
    }
}