using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace diplomska.Services
{
    public class UserApprovalService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<UserApprovalService> _logger;

        public UserApprovalService(UserManager<IdentityUser> userManager, ILogger<UserApprovalService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task ApproveSpecificUsers()
        {
            var emailsToApprove = new List<string>
            {
                "Darko.Barko@gmail.com",
                "deni.burgic@gmail.com",
                "nejc.sever@gmail.com",
                "nejc.severmihelic@gorenje.com",
                "deni.burgic@gorenje.com",
                "test@izmenovodja.com",
                "test@test.com",
                "zan.oblak@gmail.com"
            };

            var users = await _userManager.Users.ToListAsync();

            foreach (var user in users)
            {
                if (emailsToApprove.Contains(user.Email))
                {
                    var claims = await _userManager.GetClaimsAsync(user);
                    var approvedClaim = claims.FirstOrDefault(c => c.Type == "IsApproved");

                    if (approvedClaim == null)
                    {
                        await _userManager.AddClaimAsync(user, new Claim("IsApproved", "True"));
                        _logger.LogInformation($"Approved user: {user.Email}");
                    }
                }
            }
        }
    }
}
