using Microsoft.AspNetCore.Mvc;
using diplomska.Services;

namespace diplomska.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISystemLogs _systemLogs;

        public HomeController(ISystemLogs systemLogs)
        {
            _systemLogs = systemLogs;
        }

        public IActionResult Index()
        {
            _systemLogs.LogInfo("Home Controller index action invoked");
            return View();
        }
    }

}
