namespace CoffeeShop.BLL.DTOs.Inventory.Responses
{
    public class StoreInventoryDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string Unit { get; set; }
        public decimal CurrentQuantity { get; set; } 
        
    }
}