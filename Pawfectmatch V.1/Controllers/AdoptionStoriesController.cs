using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pawfectmatch_V._1.Data;

namespace Pawfectmatch_V._1.Controllers
{
    public class AdoptionStoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdoptionStoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /AdoptionStories/Gallery
        public async Task<IActionResult> Gallery(string search = null, string petType = null)
        {
            var storiesQuery = _context.AdoptionStories
                .Include(s => s.Application).ThenInclude(a => a.Pet)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                storiesQuery = storiesQuery.Where(s =>
                    (s.Application.Pet != null && s.Application.Pet.Name.Contains(search)) ||
                    (s.Application.ApplicantName != null && s.Application.ApplicantName.Contains(search)) ||
                    (s.Title != null && s.Title.Contains(search))
                );
            }
            if (!string.IsNullOrWhiteSpace(petType) && petType != "All")
            {
                storiesQuery = storiesQuery.Where(s => s.Application.Pet.PetType == petType);
            }

            var stories = await storiesQuery.OrderByDescending(s => s.DatePosted).ToListAsync();

            // Enhancement: If user is logged in, get their approved adoptions without a story
            List<Pawfectmatch_V._1.Models.AdoptionApplication> eligibleAdoptions = new();
            if (User.Identity.IsAuthenticated)
            {
                var userEmail = User.Identity.Name;
                if (!string.IsNullOrEmpty(userEmail))
                {
                    var approvedAdoptions = await _context.AdoptionApplications
                        .Include(a => a.Pet)
                        .Where(a => a.Email == userEmail && a.Status == "Approved")
                        .ToListAsync();
                    var appIdsWithStory = await _context.AdoptionStories
                        .Where(s => s.Application.Email == userEmail)
                        .Select(s => s.ApplicationId)
                        .ToListAsync();
                    eligibleAdoptions = approvedAdoptions
                        .Where(a => !appIdsWithStory.Contains(a.Id))
                        .ToList();
                }
            }
            ViewBag.EligibleAdoptions = eligibleAdoptions;
            ViewBag.Search = search;
            ViewBag.PetType = petType;
            // For filter dropdown: get all pet types in stories
            ViewBag.PetTypes = await _context.Pets.Select(p => p.PetType).Distinct().OrderBy(x => x).ToListAsync();
            return View(stories);
        }

        // GET: /AdoptionStories/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var story = await _context.AdoptionStories
                .Include(s => s.Application).ThenInclude(a => a.Pet)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (story == null) return NotFound();
            return PartialView("_StoryDetailsModal", story);
        }
    }
} 