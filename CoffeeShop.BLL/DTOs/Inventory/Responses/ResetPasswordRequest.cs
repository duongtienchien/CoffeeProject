namespace CoffeeShop.BLL.DTOs.Inventory.Responses
{
    public class ResetPasswordRequest 
    {
        public string Email { get; set; }
        public int OtpCode { get; set; }
        public string NewPassword { get; set; }
    }
}