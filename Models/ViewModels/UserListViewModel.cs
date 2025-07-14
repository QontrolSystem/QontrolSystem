using QontrolSystem.Models;

namespace QontrolSystem.Models.ViewModels
{
    public class UserListViewModel
    {
        public List<User> Users { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public string SearchString { get; set; }
        public string RoleFilter { get; set; }
        public string IsActiveFilter { get; set; }
        public string DepartmentFilter { get; set; }
    }

}
