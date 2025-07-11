using System.ComponentModel.DataAnnotations;

namespace QontrolSystem.Models.ViewModels
{
    public class ResetPasswordViewModel
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string OtpCode { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmNewPassword { get; set; }
}
}
