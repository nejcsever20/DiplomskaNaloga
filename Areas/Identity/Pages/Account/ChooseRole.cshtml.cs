using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace diplomska.Areas.Identity.Pages.Account
{
    public class ChooseRoleModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ChooseRoleModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public string SelectedRole { get; set; }

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; } = "/";

        public List<string> Roles { get; set; } = new List<string>();

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // Not logged in
                return RedirectToPage("/Account/Login");
            }

            Roles = (await _userManager.GetRolesAsync(user)).ToList();

            if (Roles == null || Roles.Count <= 1)
            {
                // If only one or no roles, redirect without showing choice
                return RedirectToPage("/Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(SelectedRole))
            {
                ModelState.AddModelError(string.Empty, "Please select a role.");
                return await OnGetAsync(); // reload roles again
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            if (!userRoles.Contains(SelectedRole))
            {
                ModelState.AddModelError(string.Empty, "Invalid role selected.");
                return await OnGetAsync();
            }

            // Redirect based on role
            return SelectedRole switch
            {
                "Admin" => RedirectToPage("/Admin/AdminPage"),
                "Skladiščnik" => RedirectToPage("/Skladiščnik/Index"),
                "Izmenovodja" => RedirectToPage("/Izmenovodja/IzmenovodjaPage"),
                "Analitika" => RedirectToPage("/Analitika/AnalitikaPage"),
                _ => LocalRedirect(ReturnUrl)
            };
        }
    }
}
