using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace diplomska.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ManageUsersModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ManageUsersModel> _logger;

        public ManageUsersModel(UserManager<IdentityUser> userManager, ILogger<ManageUsersModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public List<UserWithRoles> Users { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var users = _userManager.Users.ToList();
            Users = new List<UserWithRoles>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                Users.Add(new UserWithRoles
                {
                    User = user,
                    Roles = roles
                });
            }

            return Page();
        }

        [BindProperty]
        public string UserId { get; set; }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            if (string.IsNullOrEmpty(UserId))
                return Page();

            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                _logger.LogWarning("User not found with ID {UserId}", UserId);
                return Page();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["DeletedUser"] = true;
                return RedirectToPage();
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }

    public class UserWithRoles
    {
        public IdentityUser User { get; set; }
        public IList<string> Roles { get; set; }
    }
}
