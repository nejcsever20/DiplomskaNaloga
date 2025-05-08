using diplomska.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace diplomska.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivityController : ControllerBase
    {
        private readonly UserActivityLogger _activityLogger;
        private readonly UserManager<IdentityUser> _userManager;

        public ActivityController(UserActivityLogger activityLogger, UserManager<IdentityUser> userManager)
        {
            _activityLogger = activityLogger;
            _userManager = userManager;
        }

        [HttpPost("logUserActivity")]
        public async Task<IActionResult> LogUserActivity([FromBody] UserActivityDto activityDto)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                await _activityLogger.LogAsync(user.Id, activityDto.Action, activityDto.Type);
                return Ok();
            }

            return Unauthorized();
        }
    }

    public class UserActivityDto
    {
        public string? Action { get; set; }
        public string? Type { get; set; }
    }
}
