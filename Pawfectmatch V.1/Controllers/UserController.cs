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