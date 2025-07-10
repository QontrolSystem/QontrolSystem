namespace QontrolSystem.Models.Ticket
{
    public class Ticket
    {
        public int TicketID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Foreign Keys
        public int TicketCategoryID { get; set; }
        public int TicketStatusID { get; set; }
        public int CreatedBy { get; set; }
        public int AssignedTo { get; set; }
        public int DepartmentID { get; set; }

        // Date Fields
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public DateTime? ResolvedAt { get; set; } // Nullable because not every ticket is resolved immediately

        // Navigation Properties
        public TicketCategory TicketCategory { get; set; }
        public TicketStatus TicketStatus { get; set; }

        public User Creator { get; set; }    // CreatedBy User
        public User Assignee { get; set; }   // AssignedTo User

        public Department Department { get; set; }
    }
}

