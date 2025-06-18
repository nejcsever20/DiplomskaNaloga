using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class SetLanguageModel : PageModel
{
    public IActionResult OnPost()
    {
        // Get the culture code submitted from the form (e.g., "en-US", "fr-FR")
        var culture = Request.Form["culture"].ToString();

        if (!string.IsNullOrEmpty(culture))
        {
            // Set the culture cookie for localization with 1 year expiration
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
        }

        // Get the referring URL to redirect back to where user came from
        var returnUrl = Request.Headers["Referer"].ToString();

        // Check if the returnUrl is valid and local to prevent open redirect vulnerabilities
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        // If returnUrl is missing or not local, redirect to the homepage or a safe default page
        return RedirectToPage("/Index");
    }
}
