namespace CoffeeShop.Models.Entities.Auth
{
    public class User 
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public int FailedLoginAttempts { get; set;}
        public DateTime? LockoutEnd { get; set; }
        public int? StoreId { get; set; }
        public string OtpCode { get; set; }
        public DateTime? OtpExpiryTime { get; set; }
        public virtual Store Store { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }
}
