using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace diplomska.Pages.Admin
{
    public class EditUserModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EditUserModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public EditUserInputModel Input { get; set; }

        public List<string> AllRoles { get; set; }

        public class EditUserInputModel
        {
            public string Id { get; set; }

            [Required]
            public string UserName { get; set; }

            [Required, EmailAddress]
            public string Email { get; set; }

            [Display(Name = "New Password")]
            [DataType(DataType.Password)]
            public string NewPassword { get; set; }

            [Required]
            [Display(Name = "Role")]
            public string Role { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            // Get all roles
            AllRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            // Get user's current roles
            var roles = await _userManager.GetRolesAsync(user);

            // Bind data to the Input model
            Input = new EditUserInputModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Role = roles.FirstOrDefault()  // Default to the first role the user has
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Reload the roles to show in the select list
            AllRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            // Ensure the model state is valid
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.FindByIdAsync(Input.Id);
            if (user == null)
                return NotFound();

            // Update user details
            user.Email = Input.Email;
            user.UserName = Input.UserName;

            // Update user information
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return Page();
            }

            // Update user's role
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, Input.Role);

            // Change password if provided
            if (!string.IsNullOrEmpty(Input.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, Input.NewPassword);

                if (!passwordResult.Succeeded)
                {
                    foreach (var error in passwordResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return Page();
                }
            }

            // Redirect to the user management page after successful update
            return RedirectToPage("/Admin/ManageUsers");
        }
    }
}
