using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pawfectmatch_V._1.Data;
using Pawfectmatch_V._1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pawfectmatch_V._1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PetsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PetsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Pets
        public async Task<IActionResult> Index(string searchTerm = "", string petType = "All", string gender = "All", int page = 1)
        {
            int pageSize = 9;
            var query = _context.Pets.AsQueryable();

            // Only show available pets
            query = query.Where(p => p.Status == "Available");

            // Search
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm) || p.Breed.Contains(searchTerm) || p.Description.Contains(searchTerm));
            }

            // Filter by type
            if (!string.IsNullOrWhiteSpace(petType) && petType != "All")
            {
                query = query.Where(p => p.PetType == petType);
            }

            // Filter by gender
            if (!string.IsNullOrWhiteSpace(gender) && gender != "All")
            {
                query = query.Where(p => p.Gender == gender);
            }

            // Order by newest
            query = query.OrderByDescending(p => p.DatePosted);

            // Pagination
            var totalCount = await query.CountAsync();
            var pets = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // For filter dropdowns
            var petTypes = await _context.Pets.Select(p => p.PetType).Distinct().ToListAsync();

            // Count available cats and dogs
            var catCount = await _context.Pets.CountAsync(p => p.Status == "Available" && p.PetType == "Cat");
            var dogCount = await _context.Pets.CountAsync(p => p.Status == "Available" && p.PetType == "Dog");
            ViewBag.CatCount = catCount;
            ViewBag.DogCount = dogCount;

            ViewBag.PetTypes = petTypes;
            ViewBag.SelectedPetType = petType;
            ViewBag.SelectedGender = gender;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return View(pets);
        }

        // GET: Pets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _context.Pets
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pet == null)
            {
                return NotFound();
            }

            return View(pet);
        }

        // GET: Pets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Breed,Age,Gender,PetType,Description,ImagePath,Status,DateOfRelease,MicrochipNumber,DatePosted")] Pet pet)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pet);
        }

        // GET: Pets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _context.Pets.FindAsync(id);
            if (pet == null)
            {
                return NotFound();
            }
            return View(pet);
        }

        // POST: Pets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Breed,Age,Gender,PetType,Description,ImagePath,Status,DateOfRelease,MicrochipNumber,DatePosted")] Pet pet)
        {
            if (id != pet.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PetExists(pet.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(pet);
        }

        // GET: Pets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _context.Pets
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pet == null)
            {
                return NotFound();
            }

            return View(pet);
        }

        // POST: Pets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet != null)
            {
                _context.Pets.Remove(pet);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Pets/QuickView/5
        [AllowAnonymous]
        public async Task<IActionResult> QuickView(int id)
        {
            var pet = await _context.Pets.FirstOrDefaultAsync(p => p.Id == id && p.Status == "Available");
            if (pet == null) return NotFound();
            return PartialView("_PetQuickViewModal", pet);
        }

        private bool PetExists(int id)
        {
            return _context.Pets.Any(e => e.Id == id);
        }
    }
}
