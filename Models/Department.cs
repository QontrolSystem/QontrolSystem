namespace QontrolSystem.Models
{
    public class Department
    {
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
