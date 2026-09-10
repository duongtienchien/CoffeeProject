using CoffeeShop.Models.Entities.Auth;
namespace CoffeeShop.BLL.DTOs.Inventory.Responses
{
    public class LoginResponses {
        public string Token { get; set; }
        public string Role { get; set; }
        public int StoreId { get; set; }
    }
}