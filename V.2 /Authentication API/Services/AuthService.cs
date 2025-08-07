using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using YourApp.Data;
using YourApp.Models;
using YourApp.Models.DTOs;
using BCrypt.Net;

namespace YourApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Verify(request.Password, user.PasswordHash))
                return new AuthResponse(false, "Invalid email or password");

            var token = GenerateJwtToken(user);
            var userDto = new UserDto(user.Id, user.Name, user.Username, user.Email);

            return new AuthResponse(true, "Login successful", token, userDto);
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email || u.Username == request.Username))
                return new AuthResponse(false, "User with this email or username already exists");

            var user = new User
            {
                Name = request.Name,
                Age = request.Age,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                Username = request.Username,
                PasswordHash = BCrypt.HashPassword(request.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);
            var userDto = new UserDto(user.Id, user.Name, user.Username, user.Email);

            return new AuthResponse(true, "Registration successful", token, userDto);
        }

        public async Task<AuthResponse> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return new AuthResponse(false, "No user found with this email address");

            user.ResetToken = Guid.NewGuid().ToString("N");
            user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);

            await _context.SaveChangesAsync();

            // In production, send email here
            return new AuthResponse(true, "Password reset token generated", Data: user.ResetToken);
        }

        public async Task<AuthResponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.ResetToken == request.ResetToken &&
                u.ResetTokenExpiry > DateTime.UtcNow);

            if (user == null)
                return new AuthResponse(false, "Invalid or expired reset token");

            user.PasswordHash = BCrypt.HashPassword(request.NewPassword);
            user.ResetToken = null;
            user.ResetTokenExpiry = null;

            await _context.SaveChangesAsync();

            return new AuthResponse(true, "Password reset successful");
        }

        private string GenerateJwtToken(User user)
        {
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpiryMinutes"])),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
