#nullable disable
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using diplomska.Services; // Include your email sender service namespace

namespace diplomska.Areas.Identity.Pages.Account
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly CustomEmailSender _emailSender;

        public ConfirmEmailModel(UserManager<IdentityUser> userManager, CustomEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (!result.Succeeded)
            {
                StatusMessage = "Error confirming your email.";
                return Page();
            }

            // Auto-approval logic
            var autoApproveDomains = new[] { "@gorenje.com" };
            var autoApprovePrefixes = new[]
            {
                "darko.barko", "deni.burgic", "zan.oblak", "nejc.sever",
                "operater.com", "test2", "yourdomain", "izmenovodja"
            };

            bool shouldAutoApprove =
                autoApproveDomains.Any(domain => user.Email.EndsWith(domain, StringComparison.OrdinalIgnoreCase)) ||
                autoApprovePrefixes.Any(prefix => user.Email.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));

            string emailSubject = "Email Confirmation Completed";
            string htmlBody;

            if (shouldAutoApprove)
            {
                await _userManager.AddClaimAsync(user, new Claim("IsApproved", "True"));
                StatusMessage = "Thank you for confirming your email. Your account has been automatically approved.";

                htmlBody = $@"
                    <h3>Welcome, {user.Email}!</h3>
                    <p>Your email has been confirmed and your account is now active and approved.</p>
                    <p>You can now log in and start using the system.</p>";
            }
            else
            {
                await _userManager.AddClaimAsync(user, new Claim("IsApproved", "False"));
                StatusMessage = "Thank you for confirming your email. Your account is awaiting approval by the Izmenovodja.";

                htmlBody = $@"
                    <h3>Thank you, {user.Email}!</h3>
                    <p>Your email has been confirmed. Your account will be reviewed by the Izmenovodja shortly.</p>
                    <p>You will be notified once it's approved.</p>";
            }

            // Send confirmation email
            await _emailSender.SendCustomHtmlEmailAsync(user.Email, emailSubject, htmlBody);

            return Page();
        }
    }
}
