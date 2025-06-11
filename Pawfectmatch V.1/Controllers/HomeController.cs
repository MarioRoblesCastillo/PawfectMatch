using Microsoft.AspNetCore.Mvc;

namespace Pawfectmatch_V._1.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(); // Show the Home page
        }
        public IActionResult FindAPet()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

    }
}