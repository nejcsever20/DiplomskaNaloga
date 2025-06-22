using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace diplomska.Pages.Skladiscnik
{
    [Authorize(Roles = "Skladiščnik, Admin")]

    public class SkladiščnikPageModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
