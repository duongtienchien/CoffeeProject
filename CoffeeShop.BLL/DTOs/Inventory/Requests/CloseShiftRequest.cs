namespace CoffeeShop.BLL.DTOs.Inventory.Requests
{
    public class CloseShiftRequest
    {
        public decimal ActualCashAmount { get; set; }
        public string Note { get; set; }
    }
}