using System.ComponentModel.DataAnnotations;

namespace QontrolSystem.Models.Accounts
{
    public class PasswordResetOtp
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string OtpCode { get; set; }

        public DateTime ExpiresAt { get; set; }

        public bool IsUsed { get; set; } = false;
    }
}
