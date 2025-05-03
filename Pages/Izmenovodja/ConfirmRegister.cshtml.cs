using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using diplomska.Areas.Identity.Pages.Account;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp.Formats.Png;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace diplomska.Pages.Izmenovodja
{
    public class ConfirmRegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<ConfirmRegisterModel> _logger;

        public ConfirmRegisterModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<ConfirmRegisterModel > logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public List<IdentityUser> PendingUsers { get; set; } = new List<IdentityUser>();

        public async Task OnGetAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var pendingUsers = new List<IdentityUser>();

            foreach (var user in users)
            {
                var claims = await _userManager.GetClaimsAsync(user);
                var isApprovedClaim = claims.FirstOrDefault(c => c.Type == "IsApproved" && c.Value == "False");
                if (isApprovedClaim != null)
                {
                    pendingUsers.Add(user);
                }
            }

            PendingUsers = pendingUsers;
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Find the user by ID
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Ensure user has "Izmenovodja" role
            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Izmenovodja"))
            {
                return Forbid(); // Return 403 Forbidden if the user isn't an Izmenovodja
            }

            // Set the "IsApproved" claim to "True" to indicate the user has been approved
            var isApprovedClaim = await _userManager.GetClaimsAsync(user);
            var claim = isApprovedClaim.FirstOrDefault(c => c.Type == "IsApproved");

            if (claim != null)
            {
                await _userManager.RemoveClaimAsync(user, claim);
            }

            await _userManager.AddClaimAsync(user, new Claim("IsApproved", "True"));

            _logger.LogInformation($"User {user.Email} has been approved by Izmenovodja.");

            return RedirectToPage();
        }
    }
}
