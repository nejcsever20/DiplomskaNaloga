using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using diplomska.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace diplomska.Pages.Analitika
{
    [Authorize(Roles = "Izmenovodja, Admin, Analitika")]
    public class ConfirmRegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<ConfirmRegisterModel> _logger;
        private readonly UserApprovalService _userApprovalService;
        private readonly CustomEmailSender _customEmailSender;

        public ConfirmRegisterModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<ConfirmRegisterModel> logger,
            UserApprovalService userApprovalService,
            CustomEmailSender customEmailSender)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _userApprovalService = userApprovalService;
            _customEmailSender = customEmailSender;
        }

        public List<IdentityUser> PendingUsers { get; set; } = new List<IdentityUser>();

        public async Task OnGetAsync()
        {
            await _userApprovalService.ApproveSpecificUsers();

            var users = await _userManager.Users.ToListAsync();
            var pendingUsers = new List<IdentityUser>();

            foreach (var user in users)
            {
                var claims = await _userManager.GetClaimsAsync(user);
                if (claims.Any(c => c.Type == "IsApproved" && c.Value == "False"))
                {
                    pendingUsers.Add(user);
                }
            }

            PendingUsers = pendingUsers;
        }

        public async Task<IActionResult> OnPostApproveAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return NotFound();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            var currentRoles = await _userManager.GetRolesAsync(currentUser);
            if (!currentRoles.Contains("Izmenovodja") && !currentRoles.Contains("Admin") && !currentRoles.Contains("Analitika"))
                return Forbid();

            var claims = await _userManager.GetClaimsAsync(user);
            var isApprovedClaim = claims.FirstOrDefault(c => c.Type == "IsApproved");
            if (isApprovedClaim != null)
            {
                await _userManager.RemoveClaimAsync(user, isApprovedClaim);
                _logger.LogInformation($"Removed old IsApproved claim for {user.Email}");
            }

            await _userManager.AddClaimAsync(user, new Claim("IsApproved", "True"));
            _logger.LogInformation($"Approved user {user.Email}");

            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            await _customEmailSender.SendCustomHtmlEmailAsync(
                user.Email,
                "Your Account Has Been Approved",
                "<p>Your account has been approved by the izmenovodja. You may now log in.</p>"
            );

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeclineAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return NotFound();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            var currentRoles = await _userManager.GetRolesAsync(currentUser);
            if (!currentRoles.Contains("Izmenovodja") && !currentRoles.Contains("Admin"))
                return Forbid();

            await _customEmailSender.SendCustomHtmlEmailAsync(
                user.Email,
                "Account Registration Declined",
                "<p>Your registration has been declined by the izmenovodja.</p>"
            );

            await _userManager.DeleteAsync(user);
            _logger.LogInformation($"Declined and deleted user {user.Email}");

            return RedirectToPage();
        }
    }
}
