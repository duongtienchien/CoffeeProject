public class PasswordHasher 
{
    public string Hash(string plainPassword)
    {
        string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);
        return newPasswordHash;
    }
}