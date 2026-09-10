using CoffeeShop.DAL.Repositories;
using CoffeeShop.Models.Entities.Auth;
using CoffeeShop.BLL.Interfaces;

namespace CoffeeShop.BLL.Services
{
    public class BruteForceService : IBruteForceService
    {
        private readonly BruteForceRepository _bruteforceRepo;
        public BruteForceService(BruteForceRepository bruteforceRepo)
        {
            _bruteforceRepo = bruteforceRepo;
        }
        //Hàm check xem có tài khoản nào đang bị khoá không ?
        public async Task<bool> IsAccountLocked(User user)
        {
            //Nếu có án tích (LockoutEnd có giá trị)
            if (user.LockoutEnd.HasValue)
            {
                //Nếu giờ hiện tại nhỏ hơn giờ mãn hạn tù -> Vẫn chạy lỗi
                if (DateTime.UtcNow < user.LockoutEnd.Value)
                {
                    return true;
                }
                else
                {
                    //Nếu đã ra tù (UtcNow lớn hơn LockoutEnd)
                    user.FailedLoginAttempts = 0;
                    user.LockoutEnd = null;
                    await _bruteforceRepo.UpdateUserAttemptsAsync(user);
                    return false;
                }
            }
            return false;
        }
        //Hàm ghi nhận 1 lần sai là 1 lần cộng dồn xuống database
        public async Task CountBruteForce(User user)
        {
            user.FailedLoginAttempts += 1;
            if (user.FailedLoginAttempts >= 5)
            {
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
            }
            await _bruteforceRepo.UpdateUserAttemptsAsync(user);
        }
        public async Task ResetFailedAttemptAsync(User user)
        {
            if (user.FailedLoginAttempts > 0)
            {
                user.FailedLoginAttempts = 0;
                await _bruteforceRepo.UpdateUserAttemptsAsync(user);
            }
        }
    }
}
