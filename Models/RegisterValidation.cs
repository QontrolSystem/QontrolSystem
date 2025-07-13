using System.ComponentModel.DataAnnotations;

namespace QontrolSystem.Models
{
    // The model validates input data for user registration when the user registers in the system
    public class RegisterValidation
    {
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }


        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Email format is invalid.")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
            ErrorMessage = "Password must contain upper and lower case letters, a number, and a special character.")]
        public string PasswordHash { get; set; }


        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\d{10,}$", ErrorMessage = "Phone number must be at least 10 digits.")]
        public string PhoneNumber { get; set; }


        [Required(ErrorMessage = "Please select a department.")]
        public int DepartmentID { get; set; }

        [Display(Name = "IT Sub-Department")]
        public int? ITSubDepartmentID { get; set; }

    }
}
