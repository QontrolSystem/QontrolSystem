using QontrolSystem.Models.Accounts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QontrolSystem.Models.Ticket
{
    public class Category
    {
        [Key]
        public int TicketCategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        // Navigation property for Tickets
        [ForeignKey("ITSubDepartment")]
        public int? SubDepartmentID { get; set; }
        public ITSubDepartment ITSubDepartment { get; set; }
        public ICollection<Tickets> Tickets { get; set; } = new List<Tickets>();
        

    }
}
