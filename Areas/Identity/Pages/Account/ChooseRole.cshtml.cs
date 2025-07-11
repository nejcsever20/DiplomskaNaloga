using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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

        private static readonly Dictionary<string, string> RoleRedirects = new()
        {
            { "Admin", "/Admin/AdminPage" },
            { "Skladiščnik", "/Skladiščnik/SkladiščnikPage" },
            { "Izmenovodja", "/Izmenovodja/IzmenovodjaPage" },
            { "Analitika", "/Analitika/AnalitikaPage" },
            { "Operater", "/Operater/OperaterPage" },
            { "SBU", "/SBU/Overview" } // Example SBU route
        };

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            Roles = (await _userManager.GetRolesAsync(user)).ToList();

            if (Roles == null || Roles.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Nimate dodeljenih vlog.");
                return RedirectToPage("/Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || string.IsNullOrEmpty(SelectedRole))
            {
                ModelState.AddModelError(string.Empty, "Uporabnik ni prijavljen ali vloga ni izbrana.");
                return Page();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            if (!userRoles.Contains(SelectedRole))
            {
                ModelState.AddModelError(string.Empty, "Nimate dostopa do izbrane vloge.");
                return Page();
            }

            return await SetUserRoleAndRedirect(user, SelectedRole);
        }

        private async Task<IActionResult> SetUserRoleAndRedirect(IdentityUser user, string role)
        {
            // Sign the user out to reset session
            await _signInManager.SignOutAsync();

            // Create new claims identity
            var userPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
            var identity = (ClaimsIdentity)userPrincipal.Identity;

            // Replace or add SelectedRole claim
            var existingClaim = identity.FindFirst("SelectedRole");
            if (existingClaim != null)
            {
                identity.RemoveClaim(existingClaim);
            }

            identity.AddClaim(new Claim("SelectedRole", role));

            // Re-sign in with new claim
            await HttpContext.SignInAsync(
                IdentityConstants.ApplicationScheme,
                new ClaimsPrincipal(identity)
            );

            // Redirect based on role
            return RedirectByRole(role);
        }

        private IActionResult RedirectByRole(string role)
        {
            if (RoleRedirects.TryGetValue(role, out var targetPage))
            {
                return RedirectToPage(targetPage);
            }

            // Fallback redirect
            return LocalRedirect(ReturnUrl ?? "/");
        }
    }
}
