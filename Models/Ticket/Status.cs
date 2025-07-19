namespace QontrolSystem.Models.Ticket
{
    public class Status
    {
        public int TicketStatusID { get; set; }
        public string StatusName { get; set; } = string.Empty;
        // Navigation property for Tickets
        public ICollection<Tickets> Tickets { get; set; } = new List<Tickets>();
    }
}
