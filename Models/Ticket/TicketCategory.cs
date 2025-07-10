namespace QontrolSystem.Models.Ticket
{
    public class TicketCategory
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        // Navigation property for Tickets
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        // Override ToString() for better readability
        public override string ToString()
        {
            return $"{CategoryID}: {CategoryName}";
        }
    }
}
