namespace QontrolSystem.Models.ViewModels
{
    public class EmployeeDashboard
    {
        public int TotalTicketsLogged { get; set; }
        public int OpenTickets { get; set; }
        public int InProgressTickets { get; set; }
        public int ResolvedTickets { get; set; }
        public int ClosedTickets { get; set; }
    }
}
