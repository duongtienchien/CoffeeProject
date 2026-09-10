using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.BLL.DTOs.Inventory.Requests
{
    public class CancelOrderRequestDto
    {
        [Required(ErrorMessage = "Bắt buộc phải nhập lý do hủy đơn!")]
        [StringLength(200, ErrorMessage = "Lý do không được vượt quá 200 ký tự.")]
        public string CancellationReason { get; set; }
    }
}