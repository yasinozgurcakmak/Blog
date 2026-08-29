using EntityLayer.Concrete;

namespace Blog.Areas.blgpnl.Models
{
    public class UserWithRolesViewModel
    {
        public AppUser User { get; set; }
        public IList<string> Roles { get; set; }
    }

    public class UserRolesViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public List<RoleCheckboxItem> Roles { get; set; } 
    }

    public class RoleCheckboxItem
    {
        public string RoleName { get; set; }
        public bool IsAssigned { get; set; }
    }

}
