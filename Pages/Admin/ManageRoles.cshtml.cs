using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace diplomska.Pages.Admin
{
    public class ManageRolesModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;

        public ManageRolesModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
        }

        public List<UserWithRoles> Users { get; set; }
        public List<string> AllRoles { get; set; }

        public async Task OnGetAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            Users = new List<UserWithRoles>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                Users.Add(new UserWithRoles
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            AllRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        }

        public async Task<IActionResult> OnPostUpdateRolesAsync(string userId, List<string> Roles, List<string> RemoveRoles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);

                // Remove selected roles
                if (RemoveRoles != null)
                {
                    foreach (var role in RemoveRoles)
                    {
                        if (currentRoles.Contains(role))
                        {
                            await _userManager.RemoveFromRoleAsync(user, role);
                        }
                    }
                }

                // Add new roles
                if (Roles != null)
                {
                    foreach (var role in Roles)
                    {
                        if (!currentRoles.Contains(role))
                        {
                            await _userManager.AddToRoleAsync(user, role);
                        }
                    }
                }

                // Send email notification
                var updatedRoles = await _userManager.GetRolesAsync(user);
                string subject = "Your account roles have been updated";
                string message = $"Hello,\n\nYour roles have been updated. Your current roles are: {string.Join(", ", updatedRoles)}.\n\nIf you believe this is a mistake, please contact support.";

                await _emailSender.SendEmailAsync(user.Email, subject, message);

                return RedirectToPage();
            }

            return NotFound();
        }

        public class UserWithRoles
        {
            public string UserId { get; set; }
            public string Email { get; set; }
            public List<string> Roles { get; set; }
        }
    }
}
