using Microsoft.AspNetCore.Mvc;

namespace Pawfectmatch_V._1.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Login", "Auth", new { area = "Admin" });
        }
    }
}
