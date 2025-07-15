namespace QontrolSystem.Models.Ticket
{
    public class TicketDataTransfer
    {
        public int TicketID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int TicketCategoryID { get; set; }
        public int TicketUrgencyID { get; set; }
        public int CreatedBy { get; set; }
        public int DepartmentID { get; set; }
        public int TicketStatusID { get; set; }
        public int AssignedTo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
