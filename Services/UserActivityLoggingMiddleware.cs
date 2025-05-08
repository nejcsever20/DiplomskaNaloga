using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace diplomska.Services
{
    public class UserActivityLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly UserActivityLogger _activityLogger;
        private readonly UserManager<IdentityUser> _userManager;

        public UserActivityLoggingMiddleware(RequestDelegate next, UserActivityLogger activityLogger, UserManager<IdentityUser> userManager)
        {
            _next = next;
            _activityLogger = activityLogger;
            _userManager = userManager;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            // Check if the user is authenticated
            var user = await _userManager.GetUserAsync(httpContext.User);

            if (user != null)
            {
                // Get the path and method of the current request
                var path = httpContext.Request.Path.ToString();
                var method = httpContext.Request.Method;

                // Log the user's activity (you could change this format as needed)
                await _activityLogger.LogAsync(user.Id, $"Visited {path}", $"{method} {path}");
            }

            // Call the next middleware in the pipeline
            await _next(httpContext);
        }
    }
}