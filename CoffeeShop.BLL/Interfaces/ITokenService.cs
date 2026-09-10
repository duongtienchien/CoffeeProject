using CoffeeShop.Models.Entities.Auth;

namespace CoffeeShop.BLL.Interfaces
{
    public interface ITokenService
    {
        //Nhả ra đúng chuỗi chuỗi Token
        string GenerateJwtToken(string email, string role, int staffId, string fullName);
    }
}