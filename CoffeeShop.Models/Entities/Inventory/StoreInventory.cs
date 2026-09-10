namespace CoffeeShop.Models.Entities.Inventory
{
    public class StoreInventory 
    {
        public int StoreId { get; set; }
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public virtual Store Store { get; set; }
        // THÊM DÒNG NÀY ĐỂ NỐI VỚI BẢNG INVENTORY ITEM
        public virtual Catalog.InventoryItem InventoryItem { get; set; }
    }
}