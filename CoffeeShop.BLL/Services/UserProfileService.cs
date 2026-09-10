using CoffeeShop.DAL.Interfaces;
namespace CoffeeShop.BLL.Services
{
    public class UserProfileService
    {
        private readonly IUserRepository _userRepo;

        public UserProfileService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<List<ProfileDto>> GetStaffListAsync()
        {
            var users = await _userRepo.GetAllStaffAsync();

            return users.Select(u => new ProfileDto
            {
                UserId = u.Id,
                Username = u.Email,
                FullName = u.UserProfile?.FullName ?? "Chưa cập nhật",
                Phone = u.UserProfile?.Phone ?? "N/A",
                Avatar = u.UserProfile?.Avatar ?? "default-admin.png",
                Status = "Active"
            }).ToList();
        }
        public async Task<object> GetMyProfileAsync(string email)
        {
            var user = await _userRepo.GetUserByEmail(email);

            if (user == null) return null;

            return new
            {
                fullName = user.UserProfile?.FullName,
                role = user.Role,
                avatar = string.IsNullOrEmpty(user.UserProfile?.Avatar) ? "" : user.UserProfile.Avatar
            };
        }
    }
}