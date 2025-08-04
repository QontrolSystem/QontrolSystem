using System.ComponentModel.DataAnnotations;

namespace QontrolSystem.Models.Accounts
{
    public class Department
    {
        [Key]
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
