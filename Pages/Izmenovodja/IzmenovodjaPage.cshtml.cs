using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace diplomska.Pages.Izmenovodja
{
    [Authorize(Roles = "Admin, Izmenovodja")]

    public class IzmenovodjaPageModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
