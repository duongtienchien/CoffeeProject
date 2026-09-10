using CoffeeShop.DAL.Repositories;
using CoffeeShop.DAL.Interfaces;
using CoffeeShop.BLL.Services;
using CoffeeShop.BLL.Interfaces;
namespace CoffeeShop.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // --- Tầng Repositories ---
            services.AddScoped<AccountRepository>();
            services.AddScoped<OrderRepository>();
            services.AddScoped<ProductRepository>();
            services.AddScoped<StaffRepository>();
            services.AddScoped<BruteForceRepository>();
            services.AddScoped<ProductRecipeRepository>();
            services.AddScoped<StoreInventoryRepository>();
            services.AddScoped<InventoryRepository>();
            services.AddScoped<ManagerRepository>();
            services.AddScoped<ShiftRepository>();

            // Chỉ cần 1 dòng cho mỗi Repository
            services.AddScoped<SystemAuditLogRepository>();
            services.AddScoped<ISystemAuditLogRepository, SystemAuditLogRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            // --- Tầng Services ---
            services.AddScoped<OrderService>();
            services.AddScoped<StaffService>();
            services.AddScoped<InventoryService>();
            services.AddScoped<UserProfileService>();
            services.AddScoped<ProductService>();
            services.AddScoped<SystemAuditLogService>();
            services.AddScoped<ManagerService>();
            services.AddScoped<ShiftService>();

            services.AddScoped<TokenService>();
            services.AddScoped<ITokenService, TokenService>();

            services.AddScoped<BruteForceService>();
            services.AddScoped<IBruteForceService, BruteForceService>();
            services.AddScoped<IAuthService, AuthService>();

            // --- Utils ---
            services.AddScoped<PasswordHasher>();

            return services;
        }
    }
}
