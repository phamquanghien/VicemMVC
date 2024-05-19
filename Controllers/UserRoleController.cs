using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VicemMVC.Models.ViewModels;

namespace VicemMVC.Controllers
{
    public class UserRoleController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRoleController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var usersWithRoles = new List<UserWithRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                usersWithRoles.Add(new UserWithRolesViewModel { User = user, Roles = roles.ToList() });
            }

            return View(usersWithRoles);
        }

        public async Task<IActionResult> AssignRole(string userId)
        {
            // Lấy người dùng từ ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Lấy danh sách các vai trò của người dùng
            var userRoles = await _userManager.GetRolesAsync(user);

            // Lấy danh sách tất cả các vai trò từ cơ sở dữ liệu
            var allRoles = await _roleManager.Roles.Select(r => new RoleViewModel { Id = r.Id, Name = r.Name }).ToListAsync();

            // Tạo một đối tượng AssignRoleViewModel
            var viewModel = new AssignRoleViewModel
            {
                UserId = userId,
                AllRoles = allRoles,
                SelectedRoles = userRoles // Truyền danh sách các vai trò của người dùng vào SelectedRoles
            };

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> AssignRole(AssignRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    return NotFound();
                }

                // Lấy danh sách các vai trò hiện tại của người dùng
                var userRoles = await _userManager.GetRolesAsync(user);

                // Thêm vai trò mới cho người dùng nếu chưa có
                foreach (var role in model.SelectedRoles)
                {
                    if (!userRoles.Contains(role))
                    {
                        await _userManager.AddToRoleAsync(user, role);
                    }
                }

                // Loại bỏ vai trò không được chọn trong form
                foreach (var role in userRoles)
                {
                    if (!model.SelectedRoles.Contains(role))
                    {
                        await _userManager.RemoveFromRoleAsync(user, role);
                    }
                }

                return RedirectToAction("Index", "UserRole"); // Redirect đến trang chính sau khi thực hiện thành công
            }

            // Nếu ModelState không hợp lệ, trả về view với model để hiển thị lỗi
            return View(model);
        }
    }
}