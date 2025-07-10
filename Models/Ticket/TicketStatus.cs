namespace QontrolSystem.Models.Ticket
{
    public class TicketStatus
    {
        public int TicketStatusID { get; set; }
        public string StatusName { get; set; } = string.Empty;
        // Navigation property for Tickets
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
