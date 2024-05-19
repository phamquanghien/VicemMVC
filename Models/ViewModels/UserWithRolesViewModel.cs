namespace VicemMVC.Models.ViewModels
{
    public class UserWithRolesViewModel
    {
        public ApplicationUser User { get; set; }
        public IList<string> Roles { get; set; }
    }
}