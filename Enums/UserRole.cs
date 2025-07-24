using System.ComponentModel.DataAnnotations;

namespace QontrolSystem.Enums
{
    // List of user roles in the system
    public enum UserRole
    {
        [Display(Name = "System Administrator")]
        SystemAdministrator,

        [Display(Name = "IT Manager")]
        ITManager,

        [Display(Name = "Technician")]
        Agent, 

        [Display(Name = "Employee")]
        Employee
    }
}
