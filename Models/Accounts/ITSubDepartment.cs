using QontrolSystem.Models.Ticket;

namespace QontrolSystem.Models.Accounts
{
    public class ITSubDepartment
    {
        public int ITSubDepartmentID { get; set; }
        public string SubDepartmentName { get; set; }
        public int DepartmentID { get; set; }
        public Department Department { get; set; }
        public ICollection<Category> TicketCategories { get; set; } = new List<Category>();
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
