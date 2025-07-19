using QontrolSystem.Models.Accounts;

namespace QontrolSystem.Models.Ticket
{
    public class Tickets
    {
        public int TicketID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Foreign Keys
        public int TicketCategoryID { get; set; }
        public int TicketStatusID { get; set; }
        public int CreatedBy { get; set; }
        public int? AssignedTo { get; set; }
        public int DepartmentID { get; set; }
        public int TicketUrgencyID { get; set; }
        public int? TicketFeedbackID { get; set; } 
  

        // Date Fields
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; } 
        public DateTime? ResolvedAt { get; set; } // Nullable because not every ticket is resolved immediately

        // Navigation Properties

        public Category TicketCategory { get; set; }
        public Status TicketStatus { get; set; }

        public User Creator { get; set; }    // CreatedBy User
        public User? Assignee { get; set; }   // AssignedTo User

        public Department Department { get; set; }
        public Urgency TicketUrgency { get; set; }
        public ICollection<Attachment> TicketAttachments { get; set; } = new List<Attachment>();
        public ICollection<Feedback>? TicketFeedbacks { get; set; }
    }
}

