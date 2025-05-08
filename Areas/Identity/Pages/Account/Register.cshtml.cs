#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using diplomska.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace diplomska.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly CustomEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            CustomEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public List<SelectListItem> RoleOptions { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "Role")]
            public string Role { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            RoleOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "Admin", Text = "Admin" },
                new SelectListItem { Value = "Izmenovodja", Text = "Izmenovodja" },
                new SelectListItem { Value = "Skladiščnik", Text = "Skladiščnik" },
                new SelectListItem { Value = "Operater", Text = "Operater" }
            };
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // Ensure role exists
                    if (!await _roleManager.RoleExistsAsync(Input.Role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(Input.Role));
                    }

                    await _userManager.AddToRoleAsync(user, Input.Role);

                    // Auto-approve and auto-confirm logic
                    var autoApproveDomains = new[] { "@gorenje.com" };
                    var autoApprovePrefixes = new[]
                    {
                        "darko.barko", "deni.burgic", "zan.oblak", "nejc.sever",
                        "operater.com", "test2", "yourdomain", "izmenovodja"
                    };

                    bool shouldAutoApprove =
                        autoApproveDomains.Any(domain => user.Email.EndsWith(domain, StringComparison.OrdinalIgnoreCase)) ||
                        autoApprovePrefixes.Any(prefix => user.Email.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));

                    if (shouldAutoApprove)
                    {
                        var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        await _userManager.ConfirmEmailAsync(user, confirmToken);
                        await _userManager.AddClaimAsync(user, new Claim("IsApproved", "True"));

                        await _emailSender.SendCustomHtmlEmailAsync(user.Email, "Account Activated", $@"
                            <h3>Welcome, {user.Email}!</h3>
                            <p>Your email has been confirmed and your account has been automatically approved.</p>
                            <p>You can now log in to the system.</p>");

                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = user.Id, code = code },
                            protocol: Request.Scheme);

                        await _userManager.AddClaimAsync(user, new Claim("IsApproved", "False"));

                        await _emailSender.SendCustomHtmlEmailAsync(user.Email, "Confirm your email", $@"
                            <h3>Thank you for registering!</h3>
                            <p>Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.</p>");

                        return RedirectToPage("RegisterConfirmation", new { email = user.Email });
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Repopulate roles on error
            RoleOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "Admin", Text = "Admin" },
                new SelectListItem { Value = "Izmenovodja", Text = "Izmenovodja" },
                new SelectListItem { Value = "Skladiščnik", Text = "Skladiščnik" },
                new SelectListItem { Value = "Operater", Text = "Operater" }
            };

            return Page();
        }
    }
}
