using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var applications = _context.AdoptionApplications
                .Include(a => a.Pet)
                .OrderByDescending(a => a.SubmittedAt)
                .ToList();

            return View(applications);
        }

        public IActionResult Form(int id)
        {
            var application = _context.AdoptionApplications
                .Include(a => a.Pet)
                .FirstOrDefault(a => a.Id == id);

            if (application == null)
                return NotFound();

            return View();
        }
    }
}
