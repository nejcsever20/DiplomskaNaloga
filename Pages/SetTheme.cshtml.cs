using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace diplomska.Pages
{
    public class SetThemeModel : PageModel
    {
        [BindProperty]
        public string? Theme { get; set; }

        public IActionResult OnPost(string theme)
        {
            var validThemes = new[] { "light", "dark", "custom" };
            if (!validThemes.Contains(theme)) theme = "light";

            Response.Cookies.Append("theme", theme, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true
            });

            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
