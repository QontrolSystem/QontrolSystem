namespace QontrolSystem.Models
{
    public class ITSubDepartment
    {
        public int ITSubDepartmentID { get; set; }
        public string SubDepartmentName { get; set; }
        public int DepartmentID { get; set; }
        public Department Department { get; set; }
    }
}
