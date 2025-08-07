namespace YourApp.Models.DTOs
{
    public record LoginRequest(string Email, string Password);

    public record RegisterRequest(
        string Name,
        int Age,
        string PhoneNumber,
        string Email,
        string Username,
        string Password
    );

    public record ForgotPasswordRequest(string Email);
    public record ResetPasswordRequest(string ResetToken, string NewPassword);

    public record AuthResponse(
        bool Success,
        string Message,
        string? Token = null,
        UserDto? User = null
    );

    public record UserDto(int Id, string Name, string Username, string Email);
}