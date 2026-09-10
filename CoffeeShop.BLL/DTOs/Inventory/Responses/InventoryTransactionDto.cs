namespace CoffeeShop.BLL.DTOs.Inventory.Responses
{
    public class InventoryTransactionDto
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int QuantityChange { get; set; }
        public string TransactionType { get; set; }
        public string Reason { get; set; }
        public int CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
    }
}