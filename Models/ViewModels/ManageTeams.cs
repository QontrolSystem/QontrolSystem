using System.Collections.Generic;

namespace QontrolSystem.Models.ViewModels
{
    public class ManageTeams
    {
        public int ITSubDepartmentID { get; set; }
        public string SubDepartmentName { get; set; }
        public string DepartmentName { get; set; }
        public List<string> TechnicianNames { get; set; }
    }
}