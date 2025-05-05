using Microsoft.AspNetCore.Identity;
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

        public ManageRolesModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Change this from IdentityUser to UserWithRoles
        public List<UserWithRoles> Users { get; set; }
        public List<string> AllRoles { get; set; }

        public async Task OnGetAsync()
        {
            // Fetching the list of users with their roles asynchronously
            var users = await _userManager.Users.ToListAsync(); // Use async to fetch users
            Users = new List<UserWithRoles>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user); // Fetch roles asynchronously for each user
                Users.Add(new UserWithRoles
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Roles = roles.ToList() // Convert to list
                });
            }

            AllRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync(); // Get all available roles asynchronously
        }

        public async Task<IActionResult> OnPostUpdateRolesAsync(string userId, List<string> Roles, List<string> RemoveRoles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // Get current roles of the user
                var currentRoles = await _userManager.GetRolesAsync(user);

                // Remove roles
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

                // Return back to the page after changes
                return RedirectToPage();
            }

            // If user not found
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
