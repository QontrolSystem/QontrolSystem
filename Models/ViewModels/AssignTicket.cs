namespace QontrolSystem.Models.ViewModels
{
    public class AssignTicket
    {
        public int TicketID { get; set; }
        public int? CurrentAssigneeId { get; set; }
        public DateTime? AssignmentDate { get; set; } // you can map this to ResolvedAt or add a new scheduled field if needed
        public string? SearchTerm { get; set; }
        public List<ManageTechnicians> TeamUsers { get; set; } = new();
    }
}
