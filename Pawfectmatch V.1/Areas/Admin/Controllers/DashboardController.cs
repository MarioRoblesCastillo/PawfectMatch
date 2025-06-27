using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pawfectmatch_V._1.Data;

namespace Pawfectmatch_V._1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var totalApplications = _context.AdoptionApplications.Count(a => a.Status == "To Review");
            var totalPets = _context.Pets.Count();
            var adoptedPets = _context.Pets.Count(p => p.Status == "Adopted");

            var availablePets = totalPets - adoptedPets;  // compute available pets

            ViewBag.TotalApplications = totalApplications;
            ViewBag.TotalPets = availablePets; // ipasa na dito ang available pets count
            ViewBag.AdoptedPets = adoptedPets;

            return View();
        }

    }
}
