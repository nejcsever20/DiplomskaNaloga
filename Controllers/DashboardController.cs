using diplomska.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace diplomska.Controllers
{
    public class DashboardController : Controller
    {
        private readonly UserActivityLogger _activityLogger;
        private readonly UserManager<IdentityUser> _userManager;

        public DashboardController(UserActivityLogger activityLogger, UserManager<IdentityUser> userManager)
        {
            _activityLogger = activityLogger;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                await _activityLogger.LogAsync(user.Id, "Visited Dashboard", "GET /Dashboard");
            }

            return View();
        }
    }
}
