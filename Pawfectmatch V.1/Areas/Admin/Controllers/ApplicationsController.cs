using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pawfectmatch_V._1.Data; // <-- Add this to access ApplicationDbContext
using Pawfectmatch_V._1.Models;

namespace Pawfectmatch_V._1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApplicationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Requests()
        {
            var applications = _context.Applications.ToList(); // Load from DB
            return View(applications);
        }

        public IActionResult Form()
        {
            return View();
        }
    }
}
