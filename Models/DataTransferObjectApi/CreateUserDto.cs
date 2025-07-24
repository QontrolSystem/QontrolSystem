using System.ComponentModel.DataAnnotations;

namespace QontrolSystem.Models.DataTransferObjectApi
{
    public class CreateUserDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        [Required]
        public int DepartmentID { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public int RoleID { get; set; }
    }
}
