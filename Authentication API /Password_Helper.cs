using BCrypt.Net;

public class PasswordHelper
{
    public static string HashPassword(string password)
    {
        return BCrypt.HashPassword(password, BCrypt.GenerateSalt(12));
    }

    public static bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Verify(password, hashedPassword);
    }

    public static string GenerateResetToken()
    {
        return Guid.NewGuid().ToString("N");
    }
}
