using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Pawfectmatch_V._1.Models;

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

        public IActionResult Carousel()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    
    }
}