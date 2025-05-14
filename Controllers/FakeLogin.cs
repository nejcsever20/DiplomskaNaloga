using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace diplomska.Controllers
{
    public class FakeLogin : Controller
    {
        [Route("fake-yahoo-login")]
        public async Task<IActionResult> Index()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "yahoo user"),
                new Claim(ClaimTypes.Name, "Test Yahoo Login"),
                new Claim(ClaimTypes.Email, "testuser@yahoo.com"),
                new Claim("urm:yahoo:profile", "fake-profile-link")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home"); // or wherever you want
        }
    }
}
