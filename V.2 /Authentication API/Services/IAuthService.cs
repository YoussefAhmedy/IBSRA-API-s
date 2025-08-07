using YourApp.Models.DTOs;

namespace YourApp.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> ForgotPasswordAsync(ForgotPasswordRequest request);
        Task<AuthResponse> ResetPasswordAsync(ResetPasswordRequest request);
    }
}

