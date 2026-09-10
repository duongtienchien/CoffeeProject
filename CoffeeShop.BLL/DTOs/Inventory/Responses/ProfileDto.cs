public class ProfileDto
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string FullName { get; set; }
    public string Phone { get; set; }
    public string Avatar { get; set; } 
    public string Status { get; set; }
    public decimal SystemCashAmount { get; set; }
    public decimal ActualCashAmount { get; set; }
    public decimal Difference { get; set; }
}