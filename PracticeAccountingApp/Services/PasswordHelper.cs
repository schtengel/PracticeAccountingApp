// Services/PasswordHelper.cs
public static class PasswordHelper
{
    public static string HashPassword(string password) =>
        BCrypt.Net.BCrypt.EnhancedHashPassword(password, 11);

    public static bool VerifyPassword(string password, string hash) =>
        BCrypt.Net.BCrypt.EnhancedVerify(password, hash);
}