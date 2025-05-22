using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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
        private readonly IEmailSender _emailSender;

        public EditUserModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
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

            AllRoles = _roleManager.Roles.Select(r => r.Name).ToList();
            var roles = await _userManager.GetRolesAsync(user);

            Input = new EditUserInputModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Role = roles.FirstOrDefault()
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            AllRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.FindByIdAsync(Input.Id);
            if (user == null)
                return NotFound();

            user.Email = Input.Email;
            user.UserName = Input.UserName;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return Page();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, Input.Role);

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

                // Send email notification about password change
                string subject = "Your password has been changed";
                string message = $"Hello,\n\nYour password was recently changed by an administrator.\n\nIf you did not request this change, please contact support immediately.";

                await _emailSender.SendEmailAsync(user.Email, subject, message);
            }

            return RedirectToPage("/Admin/ManageUsers");
        }
    }
}
