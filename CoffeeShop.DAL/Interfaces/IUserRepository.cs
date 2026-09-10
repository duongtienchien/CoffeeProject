using CoffeeShop.Models.Entities.Auth;
namespace CoffeeShop.DAL.Interfaces
{
    public interface IUserRepository
    {
        //Có nhiệm vụ lấy email và trả về thông tin người dùng dùng (nếu có)
        Task<User> GetUserByEmail(string email);
        Task<List<User>> GetAllStaffAsync();
    }
}