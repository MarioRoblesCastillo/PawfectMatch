using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pawfectmatch_V._1.Data;
using Pawfectmatch_V._1.Models;

namespace Pawfectmatch_V._1.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var applications = await _context.AdoptionApplications
                .Include(a => a.Pet)
                .Where(a => a.Email == user.Email && a.Status != "Draft")
                .OrderByDescending(a => a.SubmittedAt)
                .ToListAsync();

            var draftApplications = await _context.AdoptionApplications
                .Include(a => a.Pet)
                .Where(a => a.Email == user.Email && a.Status == "Draft")
                .OrderByDescending(a => a.SubmittedAt)
                .ToListAsync();

            var stories = await _context.AdoptionStories
                .Include(s => s.Application)
                .Where(s => s.Application.Email == user.Email)
                .ToListAsync();

            ViewBag.MyStories = stories;
            ViewBag.DraftApplications = draftApplications;
            return View(applications);
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ApplicationUser model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                // Add more fields as needed
                await _userManager.UpdateAsync(user);
                ViewBag.SuccessMessage = "Profile updated successfully!";
            }
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> ShareStory(int applicationId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // Enhancement: If user has no approved adoptions, redirect with message
            var hasApproved = await _context.AdoptionApplications.AnyAsync(a => a.Email == user.Email && a.Status == "Approved");
            if (!hasApproved)
            {
                TempData["InfoMessage"] = "You need to have an approved adoption before sharing a story. Start your adoption journey below!";
                return RedirectToAction("Index", "ApplicationForm");
            }

            var app = await _context.AdoptionApplications.Include(a => a.Pet)
                .FirstOrDefaultAsync(a => a.Id == applicationId && a.Email == user.Email && a.Status == "Approved");
            if (app == null) return NotFound();

            // Only allow if no story exists yet
            var existing = await _context.AdoptionStories.FirstOrDefaultAsync(s => s.ApplicationId == applicationId);
            if (existing != null) return RedirectToAction("Dashboard");

            var model = new AdoptionStory { ApplicationId = applicationId };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShareStory(AdoptionStory model, IFormFile? PhotoFile)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var app = await _context.AdoptionApplications.Include(a => a.Pet)
                .FirstOrDefaultAsync(a => a.Id == model.ApplicationId && a.Email == user.Email && a.Status == "Approved");
            if (app == null) return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            // Only allow if no story exists yet
            var existing = await _context.AdoptionStories.FirstOrDefaultAsync(s => s.ApplicationId == model.ApplicationId);
            if (existing != null) return RedirectToAction("Dashboard");

            if (PhotoFile != null && PhotoFile.Length > 0)
            {
                var uploads = Path.Combine("wwwroot", "uploads");
                Directory.CreateDirectory(uploads);
                var fileName = $"story_{Guid.NewGuid()}{Path.GetExtension(PhotoFile.FileName)}";
                var filePath = Path.Combine(uploads, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await PhotoFile.CopyToAsync(stream);
                }
                model.PhotoPath = "/uploads/" + fileName;
            }

            model.DatePosted = DateTime.Now;
            model.IsApproved = true;
            _context.AdoptionStories.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Thank you for sharing your story! It will appear in the gallery once approved.";
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public async Task<IActionResult> EditStory(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");
            var story = await _context.AdoptionStories.Include(s => s.Application)
                .FirstOrDefaultAsync(s => s.Id == id && s.Application.Email == user.Email);
            if (story == null) return NotFound();
            return View(story);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStory(AdoptionStory model, IFormFile? PhotoFile)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");
            var story = await _context.AdoptionStories.Include(s => s.Application)
                .FirstOrDefaultAsync(s => s.Id == model.Id && s.Application.Email == user.Email);
            if (story == null) return NotFound();
            if (!ModelState.IsValid) return View(model);

            story.Title = model.Title;
            story.StoryText = model.StoryText;
            story.DatePosted = DateTime.Now;
            if (PhotoFile != null && PhotoFile.Length > 0)
            {
                var uploads = Path.Combine("wwwroot", "uploads");
                Directory.CreateDirectory(uploads);
                var fileName = $"story_{Guid.NewGuid()}{Path.GetExtension(PhotoFile.FileName)}";
                var filePath = Path.Combine(uploads, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await PhotoFile.CopyToAsync(stream);
                }
                story.PhotoPath = "/uploads/" + fileName;
            }
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Your story has been updated and will be re-reviewed.";
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStory(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");
            var story = await _context.AdoptionStories.Include(s => s.Application)
                .FirstOrDefaultAsync(s => s.Id == id && s.Application.Email == user.Email);
            if (story == null) return NotFound();
            _context.AdoptionStories.Remove(story);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Your story has been deleted.";
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDraft(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");
            var draft = await _context.AdoptionApplications.FirstOrDefaultAsync(a => a.Id == id && a.Status == "Draft" && a.Email == user.Email);
            if (draft == null)
            {
                TempData["ErrorMessage"] = "Draft not found or already submitted.";
                return RedirectToAction("Dashboard");
            }
            _context.AdoptionApplications.Remove(draft);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Draft deleted successfully.";
            return RedirectToAction("Dashboard");
        }
    }
} 