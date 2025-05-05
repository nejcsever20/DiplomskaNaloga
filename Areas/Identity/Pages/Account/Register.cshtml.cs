using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using diplomska.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace diplomska.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly CustomEmailSender _customEmailSender;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            RoleManager<IdentityRole> roleManager,
            CustomEmailSender customEmailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
            _customEmailSender = customEmailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Compare("Password")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string Role { get; set; }  // Role selection
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    // Ensure the role exists
                    if (!await _roleManager.RoleExistsAsync(Input.Role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(Input.Role));
                    }

                    // Assign role
                    await _userManager.AddToRoleAsync(user, Input.Role);

                    // Add claim to mark user as unapproved
                    await _userManager.AddClaimAsync(user, new Claim("IsApproved", "False"));

                    _logger.LogInformation("User created a new account with password but requires approval.");

                    await _customEmailSender.SendCustomHtmlEmailAsync(
                        Input.Email,
                        "Registration Successful",
                        "<p>You have successfully registered. Please wait for the izmenovodja to confirm your account.</p>"
                    );

                    // DO NOT sign in user immediately. Wait for Izmenovodja to approve.
                    return RedirectToPage("RegisterConfirmation");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed
            return Page();
        }

    }
}
