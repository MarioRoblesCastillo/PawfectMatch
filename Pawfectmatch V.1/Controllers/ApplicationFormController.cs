using Microsoft.AspNetCore.Mvc;

namespace Pawfectmatch_V._1.Controllers
{
    public class ApplicationFormController : Controller
    {
        // Show the application form
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Handle form submission (frontend only, no backend logic)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(
            string FullName,
            string Email,
            string Phone,
            string Message)
        {
            // You can add logic here to show a "Thank you" message or validation errors
            ViewBag.Status = "pending"; // Example: set status for frontend display
            return View();
        }
    }
}