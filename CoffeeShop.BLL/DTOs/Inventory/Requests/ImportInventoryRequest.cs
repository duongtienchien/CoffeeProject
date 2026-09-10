namespace CoffeeShop.BLL.DTOs.Inventory.Requests
{
    public class ImportInventoryRequest
    {
        public int ItemId { get; set; }
        public int QuantityToAdd { get; set; }
    }
}