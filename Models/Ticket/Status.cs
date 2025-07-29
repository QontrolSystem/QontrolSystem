using System.ComponentModel.DataAnnotations;

namespace QontrolSystem.Models.Ticket
{
    public class Status
    {
        [Key]
        public int TicketStatusID { get; set; }

        public string StatusName { get; set; }

        // Navigation property for Tickets
        public ICollection<Tickets> Tickets { get; set; } = new List<Tickets>();
    }
}
