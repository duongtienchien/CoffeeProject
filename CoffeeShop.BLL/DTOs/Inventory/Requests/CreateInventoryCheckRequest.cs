namespace CoffeeShop.BLL.DTOs.Inventory.Requests
{
    public class CreateInventoryCheckRequest
    {
        public int StoreId { get; set; }
        public int ItemId { get; set; }
        public decimal ActualQuantity { get; set; }
    }
}