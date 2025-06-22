using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace diplomska.Pages.Analitik
{
    [Authorize(Roles = "Admin, Analitika")]

    public class AnalitikPageModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
