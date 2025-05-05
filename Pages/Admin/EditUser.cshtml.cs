using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace diplomska.Pages.Admin
{
    public class EditUserModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public EditUserModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public EditUserInputModel Input { get; set; }

        public class EditUserInputModel
        {
            public string Id { get; set; }

            [Required, EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            Input = new EditUserInputModel
            {
                Id = user.Id,
                Email = user.Email
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByIdAsync(Input.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.Email = Input.Email;
            user.UserName = Input.Email;  // Ensure the username is updated to match the new email.

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return Page();
            }

            return RedirectToPage("/Admin/ManageUsers");
        }
    }
}
