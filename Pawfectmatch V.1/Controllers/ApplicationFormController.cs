﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pawfectmatch_V._1.Data;
using Pawfectmatch_V._1.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Pawfectmatch_V._1.Controllers
{
    public class ApplicationFormController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationFormController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Show the application form (optionally load a draft by id or for a specific pet)
        [HttpGet]
        public async Task<IActionResult> Index(int? id, int? petId)
        {
            // Always show dropdown for pet selection
            var availablePets = await _context.Pets.Where(p => p.Status == "Available").OrderBy(p => p.Name).ToListAsync();
            ViewBag.AvailablePets = availablePets;
            return View(new AdoptionApplication());
        }

        // Handle form submission with proper backend logic
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(AdoptionApplication application, string saveDraft)
        {
            // If saving as draft, skip validation and save as 'Draft'
            if (!string.IsNullOrEmpty(saveDraft))
            {
                application.Status = "Draft";
                application.SubmittedAt = DateTime.Now;
                _context.AdoptionApplications.Add(application);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Your application has been saved as a draft. You can finish it later from your dashboard.";
                ViewBag.AvailablePets = await _context.Pets.Where(p => p.Status == "Available").OrderBy(p => p.Name).ToListAsync();
                return View(application);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Set default values
                    application.Status = "To Review";
                    application.SubmittedAt = DateTime.Now;

                    // Validate that the pet exists and is available
                    var pet = await _context.Pets.FindAsync(application.PetId);
                    if (pet == null)
                    {
                        ModelState.AddModelError("PetId", "Selected pet not found.");
                        ViewBag.AvailablePets = await _context.Pets.Where(p => p.Status == "Available").OrderBy(p => p.Name).ToListAsync();
                        return View(application);
                    }

                    if (pet.Status != "Available")
                    {
                        ModelState.AddModelError("PetId", "Selected pet is not available for adoption.");
                        ViewBag.AvailablePets = await _context.Pets.Where(p => p.Status == "Available").OrderBy(p => p.Name).ToListAsync();
                        return View(application);
                    }

                    // Add to database
                    _context.AdoptionApplications.Add(application);
                    await _context.SaveChangesAsync();

                    // Debug: Show last 5 application statuses
                    var recent = _context.AdoptionApplications
                        .OrderByDescending(a => a.SubmittedAt)
                        .Take(5)
                        .Select(a => a.Status + ": " + a.ApplicantName)
                        .ToList();
                    TempData["DebugInfo"] = "Recent: " + string.Join(", ", recent);

                    TempData["SuccessMessage"] = "You've submitted it. Thank you.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while submitting your application. Please try again.");
                    // Log the exception in a production environment
                    ViewBag.AvailablePets = await _context.Pets.Where(p => p.Status == "Available").OrderBy(p => p.Name).ToListAsync();
                    return View(application);
                }
            }

            // If we get here, there was an error - reload available pets
            ViewBag.AvailablePets = await _context.Pets
                .Where(p => p.Status == "Available")
                .OrderBy(p => p.Name)
                .ToListAsync();
            return View(application);
        }

        // API endpoint for getting available pets (for AJAX calls)
        [HttpGet]
        public async Task<IActionResult> GetAvailablePets()
        {
            var pets = await _context.Pets
                .Where(p => p.Status == "Available")
                .Select(p => new { p.Id, p.Name, p.Breed, p.PetType, p.Age, p.Gender, p.ImagePath })
                .ToListAsync();

            return Json(pets);
        }
    }
}