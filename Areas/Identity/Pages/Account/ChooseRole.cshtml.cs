using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace diplomska.Areas.Identity.Pages.Account
{
    public class ChooseRoleModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public ChooseRoleModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
                return RedirectToPage("/Account/Login");
            }

            Roles = (await _userManager.GetRolesAsync(user)).ToList();

            if (Roles == null || Roles.Count <= 1)
            {
                if (Roles.Count == 1)
                {
                    return await SetUserRoleAndRedirect(user, Roles[0]);
                }

                return RedirectToPage("/Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || string.IsNullOrEmpty(SelectedRole))
            {
                return RedirectToPage("/Index");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            if (!userRoles.Contains(SelectedRole))
            {
                ModelState.AddModelError(string.Empty, "You do not have access to this role.");
                return Page();
            }

            return await SetUserRoleAndRedirect(user, SelectedRole);
        }

        private async Task<IActionResult> SetUserRoleAndRedirect(IdentityUser user, string role)
        {
            // Sign the user out
            await _signInManager.SignOutAsync();

            // Create new identity with SelectedRole
            var userPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
            var identity = (ClaimsIdentity)userPrincipal.Identity;

            // Remove old SelectedRole claim if exists
            var existingClaim = identity.FindFirst("SelectedRole");
            if (existingClaim != null)
            {
                identity.RemoveClaim(existingClaim);
            }

            // Add new SelectedRole claim
            identity.AddClaim(new Claim("SelectedRole", role));

            // Sign in with the updated claims
            await HttpContext.SignInAsync(
                IdentityConstants.ApplicationScheme,
                new ClaimsPrincipal(identity)
            );

            // Redirect based on role
            return role switch
            {
                "Admin" => RedirectToPage("/Admin/AdminPage"),
                "Skladiščnik" => RedirectToPage("/Skladiščnik/SkladiščnikPage"),
                "Izmenovodja" => RedirectToPage("/Izmenovodja/IzmenovodjaPage"),
                "Analitika" => RedirectToPage("/Analitika/AnalitikaPage"),
                _ => LocalRedirect(ReturnUrl)
            };
        }
    }
}
