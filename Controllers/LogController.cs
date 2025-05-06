using Microsoft.AspNetCore.Mvc;

namespace diplomska.Controllers
{
    public class LogController : Controller
    {
        public IActionResult SystemLogs()
        {
            // This will look for the SystemLogs view in the Services folder
            return View("~/Services/SystemLogs.cshtml");
        }
    }
}