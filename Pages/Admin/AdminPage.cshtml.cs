using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace diplomska.Pages.Admin
{
    [Authorize (Roles="Admin")]
    public class AdminPageModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
