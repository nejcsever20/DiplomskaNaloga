using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using diplomska.Services;
using Microsoft.Identity.Client; // Make sure to include this for UserApprovalService

namespace diplomska.Pages.Izmenovodja
{
    public class ConfirmRegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<ConfirmRegisterModel> _logger;
        private readonly UserApprovalService _userApprovalService; // Inject the approval service
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
            _userApprovalService = userApprovalService; // Initialize the service
            _customEmailSender = customEmailSender;
        }

        public List<IdentityUser> PendingUsers { get; set; } = new List<IdentityUser>();

        public async Task OnGetAsync()
        {
            // Approve specific users first
            await _userApprovalService.ApproveSpecificUsers();

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
                _logger.LogWarning("User not found with ID: " + id); // Log user not found
                return NotFound();
            }

            // Ensure user has "Izmenovodja" role
            var currentUser = await _userManager.GetUserAsync(User);
            var currentRoles = await _userManager.GetRolesAsync(currentUser);
            if (!currentRoles.Contains("Izmenovodja"))
            {
                _logger.LogWarning("User is not authorized to approve: " + currentUser.Email);
                return Forbid();
            }

            // Set the "IsApproved" claim to "True"
            var isApprovedClaim = await _userManager.GetClaimsAsync(user);
            var claim = isApprovedClaim.FirstOrDefault(c => c.Type == "IsApproved");

            if (claim != null)
            {
                await _userManager.RemoveClaimAsync(user, claim);
                _logger.LogInformation($"Removed previous approval claim for user {user.Email}");
            }

            await _userManager.AddClaimAsync(user, new Claim("IsApproved", "True"));
            _logger.LogInformation($"User {user.Email} has been approved by Izmenovodja.");

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostApproveUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user != null)
            {
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);

                await _customEmailSender.SendCustomHtmlEmailAsync(
                    user.Email,
                    "Account Approved",
                    "<p>Your account has been approved by the izmenovodja. You may now log in.</p>"
                );
            }

            return RedirectToPage();
        }
    }
}
