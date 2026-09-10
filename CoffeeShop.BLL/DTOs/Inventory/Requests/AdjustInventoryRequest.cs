namespace CoffeeShop.BLL.DTOs.Inventory.Requests
{
    public class AdjustInventoryRequest
    {
        public int ItemId { get; set; }
        
        // Số âm (trừ hao hụt) hoặc số dương (nhập sót)
        public int QuantityChange { get; set; } 
        
        // Bắt buộc phải có lý do (VD: "Trừ hao hụt do Staff ABC nhập sai")
        public string Reason { get; set; } 
    }
}