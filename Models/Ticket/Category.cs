namespace QontrolSystem.Models.Ticket
{
    public class Category
    {
        public int TicketCategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        // Navigation property for Tickets
        public ICollection<Tickets> Tickets { get; set; } = new List<Tickets>();
        

    }
}
