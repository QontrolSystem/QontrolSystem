using System.ComponentModel.DataAnnotations;

namespace QontrolSystem.Models.Ticket
{
    public class Category
    {
        [Key]
        public int TicketCategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        // Navigation property for Tickets
        public ICollection<Tickets> Tickets { get; set; } = new List<Tickets>();
        

    }
}
