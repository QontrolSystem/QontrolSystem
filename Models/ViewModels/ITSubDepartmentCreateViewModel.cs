using System.ComponentModel.DataAnnotations;

namespace QontrolSystem.Models.Accounts
{
    public class ITSubDepartmentCreateViewModel
    {
        [Required(ErrorMessage = "Sub-Department Name is required.")]
        public string SubDepartmentName { get; set; }

        [Required]
        public int DepartmentID { get; set; }
    }
}
