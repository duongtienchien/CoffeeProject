using CoffeeShop.Models.Entities.Auth;
using CoffeeShop.DAL.Data;
namespace CoffeeShop.DAL.Repositories
{
    public class BruteForceRepository
    {
        private readonly AppDbContext _dbContext;
        public BruteForceRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<int> UpdateUserAttemptsAsync(User user)
        {
            //Bắt buộc phải có dòng Update này
            _dbContext.Users.Update(user);
            return await _dbContext.SaveChangesAsync();
        }
    }
}
