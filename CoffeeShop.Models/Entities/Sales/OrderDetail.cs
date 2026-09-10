namespace CoffeeShop.Models.Entities.Sales
{
    public class OrderDetail
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        /// ID của ly nước chính. Dùng để đính kèm Topping (Trân châu, Kem cheese...) vào đúng ly nước này.
        /// Nếu là null -> Đây là ly nước chính. Nếu có giá trị -> Đây là Topping.

        public Guid? ParentOrderDetailId { get; set; }
        public virtual Order Order { get; set; }
        // THÊM DÒNG NÀY ĐỂ LẤY TÊN VÀ GIÁ SẢN PHẨM SAU NÀY
        public virtual Catalog.Product Product { get; set; }
    }
}