using System.ComponentModel.DataAnnotations;

namespace YourApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public int Age { get; set; }

        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; }

        [Required, MaxLength(100)]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}