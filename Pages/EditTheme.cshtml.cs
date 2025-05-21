using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace diplomska.Pages
{
    public class EditThemeModel : PageModel
    {
        [BindProperty]
        public string? Theme { get; set; }

        public string? CurrentTheme { get; set; }

        public void OnGet()
        {
            CurrentTheme = Request.Cookies["theme"] ?? "light";
        }

        public IActionResult OnPost(string theme)
        {
            if (theme != "light" && theme != "dark")
                theme = "light";

            Response.Cookies.Append("theme", theme, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true
            });

            return RedirectToPage(); // Refresh to show new theme
        }

        public IActionResult OnPostSetTheme(string theme)
        {
            Response.Cookies.Append("theme", theme, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                HttpOnly = false
            });

            return RedirectToPage("/Index"); // or wherever you want to go after setting the theme
        }

    }
}
