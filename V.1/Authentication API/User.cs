using System;

public class User
{
    public int ID { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string ResetToken { get; set; }
    public DateTime? ResetTokenExpiry { get; set; }
    public DateTime CreatedAt { get; set; }
}
