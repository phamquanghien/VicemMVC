using Microsoft.AspNetCore.Identity;

namespace VicemMVC.Models.ViewModels
{
    public class AssignRoleViewModel
    {
        public string UserId { get; set; }
        public IList<string> SelectedRoles { get; set; } // Danh sách các vai trò được chọn

        // Danh sách tất cả các vai trò
        public IList<RoleViewModel>? AllRoles { get; set; }
    }

    public class RoleViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}