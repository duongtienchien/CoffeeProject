using CoffeeShop.BLL.Interfaces;
using CoffeeShop.DAL.Interfaces;
using CoffeeShop.BLL.DTOs.Inventory.Requests;
using CoffeeShop.BLL.DTOs.Inventory.Responses;
using Microsoft.Extensions.Configuration;

namespace CoffeeShop.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IBruteForceService _bruteForceService;

        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, ITokenService tokenService, IBruteForceService bruteForceService, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _bruteForceService = bruteForceService;
            _configuration = configuration;
        }

        //Trả về ĐÚNG kiểu LoginResponses để khớp với Interface!
        public async Task<LoginResponses> Login(LoginRequests request)
        {
            var userInDb = await _userRepository.GetUserByEmail(request.Email);

            // 2. Không tìm thấy User trong Database -> Đuổi về
            if (userInDb == null)
            {
                return null;
            }
            if (await _bruteForceService.IsAccountLocked(userInDb))
            {
                throw new UnauthorizedAccessException("Tài khoản đã bị khóa 15 phút do nhập sai quá 5 lần. Vui lòng thử lại sau!");
            }
            bool isMatch = BCrypt.Net.BCrypt.Verify(request.Password, userInDb.PasswordHash);
            // 3. So sánh Password truyền vào với PasswordHash lấy từ Database lên
            if (!isMatch)
            {
                await _bruteForceService.CountBruteForce(userInDb);
                throw new UnauthorizedAccessException("Sai mật khẩu!");
            }
            await _bruteForceService.ResetFailedAttemptAsync(userInDb);
            var fullName = userInDb.UserProfile?.FullName ?? "Chưa cập nhật tên";
            string realToken = _tokenService.GenerateJwtToken(userInDb.Email, userInDb.Role, userInDb.Id, fullName);
            return new LoginResponses
            {
                Role = userInDb.Role,
                Token = realToken,
                StoreId = userInDb.StoreId ?? 1
            };
        }
    }
}