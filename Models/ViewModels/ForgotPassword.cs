using System.ComponentModel.DataAnnotations;

namespace QontrolSystem.Models.ViewModels
{
    public class ForgotPassword
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
