using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pawfectmatch_V._1.Models;
using Pawfectmatch_V._1.Data;
using Pawfectmatch_V._1.Helpers;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace Pawfectmatch_V._1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Get featured pets for the home page
            var featuredPets = _context.Pets
                .Where(p => p.Status == "Available")
                .OrderByDescending(p => p.DatePosted)
                .Take(6)
                .ToList();

            ViewBag.FeaturedPets = featuredPets;
            return View();
        }

        public async Task<IActionResult> FindAPet(string searchTerm = "", string petType = "All", string gender = "All", int page = 1)
        {
            int pageSize = 12;

            var query = _context.Pets
                .Where(p => p.Status == "Available")
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm) || 
                                        p.Breed.Contains(searchTerm) || 
                                        p.Description.Contains(searchTerm));
            }

            // Apply pet type filter
            if (!string.IsNullOrEmpty(petType) && petType != "All")
            {
                query = query.Where(p => p.PetType == petType);
            }

            // Apply gender filter
            if (!string.IsNullOrEmpty(gender) && gender != "All")
            {
                query = query.Where(p => p.Gender == gender);
            }

            // Order by date posted (newest first)
            query = query.OrderByDescending(p => p.DatePosted);

            var paginatedPets = await PaginatedList<Pet>.CreateAsync(query, page, pageSize);

            ViewBag.SearchTerm = searchTerm;
            ViewBag.SelectedPetType = petType;
            ViewBag.SelectedGender = gender;

            // Get unique pet types for filter dropdown
            ViewBag.PetTypes = await _context.Pets
                .Where(p => p.Status == "Available")
                .Select(p => p.PetType)
                .Distinct()
                .ToListAsync();

            return View(paginatedPets);
        }

        // GET: Home/PetDetails/5
        public async Task<IActionResult> PetDetails(int? id)
        {
            if (id == null)
                return NotFound();

            var pet = await _context.Pets
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pet == null)
                return NotFound();

            // Get related pets (same type, different pets)
            var relatedPets = await _context.Pets
                .Where(p => p.PetType == pet.PetType && 
                           p.Id != pet.Id && 
                           p.Status == "Available")
                .Take(4)
                .ToListAsync();

            ViewBag.RelatedPets = relatedPets;

            return View(pet);
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

        // API endpoint for AJAX pet search
        [HttpGet]
        public async Task<IActionResult> SearchPets(string term)
        {
            if (string.IsNullOrEmpty(term))
                return Json(new List<object>());

            var pets = await _context.Pets
                .Where(p => p.Status == "Available" && 
                           (p.Name.Contains(term) || p.Breed.Contains(term)))
                .Select(p => new { p.Id, p.Name, p.Breed, p.PetType, p.Age, p.Gender, p.ImagePath })
                .Take(10)
                .ToListAsync();

            return Json(pets);
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl = null)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return Redirect(Request.Headers["Referer"].ToString() ?? "/");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}