using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pawfectmatch_V._1.Data;
using Pawfectmatch_V._1.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using Rotativa.AspNetCore;
using ClosedXML.Excel;
using System.IO;
using Pawfectmatch_V._1.Helpers;


namespace Pawfectmatch_V._1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PetsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        private string CapitalizeWords(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;

            var words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return string.Join(' ', words.Select(word =>
                char.ToUpper(word[0]) + word.Substring(1).ToLower()
            ));
        }

        public PetsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }


        // GET: Admin/Pets/ForAdoption
        public async Task<IActionResult> ForAdoption(string petType = "Cat", int page = 1)
        {
            int pageSize = 10;

            var petsQuery = _context.Pets
                .Where(p => p.Status == "Available" && p.PetType == petType)
                .OrderByDescending(p => p.Id)
                .AsNoTracking();

            var paginatedPets = await PaginatedList<Pet>.CreateAsync(petsQuery, page, pageSize);

            ViewData["CurrentPetType"] = petType; // To remember current tab

            return View(paginatedPets);
        }


        // GET: Admin/Pets/Adopted

        public async Task<IActionResult> Adopted(int page = 1)
        {
            int pageSize = 6;

            var petsQuery = _context.Pets
                .Where(p => p.Status == "Adopted")
                .OrderByDescending(p => p.DateOfRelease) // Sort by adoption date, latest first
                .AsNoTracking();

            var paginatedPets = await PaginatedList<Pet>.CreateAsync(petsQuery, page, pageSize);

            return View(paginatedPets);
        }

        // GET: Admin/Pets/Create
        public IActionResult Create()
        {
            return View(); // Don't prefill MicrochipNumber here
        }

        // POST: Admin/Pets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pet pet, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                pet.DatePosted = DateTime.Now;
                pet.Name = CapitalizeWords(pet.Name);
                pet.Breed = CapitalizeWords(pet.Breed);
                pet.Description = CapitalizeWords(pet.Description);
                pet.Gender = CapitalizeWords(pet.Gender);
                pet.Status = CapitalizeWords(pet.Status);

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var uploads = Path.Combine(_env.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploads);
                    var filePath = Path.Combine(uploads, ImageFile.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }
                    pet.ImagePath = "/uploads/" + ImageFile.FileName;
                }
                else
                {
                    pet.ImagePath = "/uploads/default.jpg";
                }

                // First save to get the ID
                _context.Add(pet);
                await _context.SaveChangesAsync();

                // Now generate and update the microchip number
                pet.MicrochipNumber = $"MC-{pet.Id.ToString().PadLeft(4, '0')}";
                _context.Update(pet);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"New pet added successfully! Microchip #: {pet.MicrochipNumber}";
                return RedirectToAction(nameof(ForAdoption));
            }

            return View(pet); // If validation fails
        }


        // GET: Admin/Pets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var pet = await _context.Pets
                .Include(p => p.PetImages)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (pet == null) return NotFound();

            return View(pet);
        }

        // POST: Admin/Pets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Pet pet, IFormFile[] ImageFiles)
        {
            if (id != pet.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var existingPet = await _context.Pets.Include(p => p.PetImages).FirstOrDefaultAsync(p => p.Id == id);
                if (existingPet == null)
                    return NotFound();

                try
                {
                    // Update fields manually
                    existingPet.Name = CapitalizeWords(pet.Name);
                    existingPet.PetType = CapitalizeWords(pet.PetType);
                    existingPet.Breed = CapitalizeWords(pet.Breed);
                    existingPet.Description = CapitalizeWords(pet.Description);
                    existingPet.Gender = CapitalizeWords(pet.Gender);
                    existingPet.Status = CapitalizeWords(pet.Status);
                    existingPet.Age = pet.Age;
                    _context.Entry(existingPet).Property(p => p.Age).IsModified = true;

                    if (existingPet.Status == "Adopted" && existingPet.DateOfRelease == null)
                    {
                        existingPet.DateOfRelease = DateTime.Now;
                    }

                    // Handle multiple image uploads
                    if (ImageFiles != null && ImageFiles.Length > 0)
                    {
                        var uploads = Path.Combine(_env.WebRootPath, "uploads");
                        Directory.CreateDirectory(uploads);
                        foreach (var file in ImageFiles)
                        {
                            if (file != null && file.Length > 0)
                            {
                                var fileName = $"pet_{existingPet.Id}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                                var filePath = Path.Combine(uploads, fileName);
                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }
                                var image = new PetImage
                                {
                                    PetId = existingPet.Id,
                                    ImagePath = "/uploads/" + fileName,
                                    SortOrder = existingPet.PetImages.Count
                                };
                                _context.PetImages.Add(image);
                            }
                        }
                    }

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Pet updated successfully!";
                    return RedirectToAction(nameof(ForAdoption));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Pets.Any(e => e.Id == pet.Id))
                        return NotFound();
                    else
                        throw;
                }
            }

            // If we get here, something was invalid
            // Reload images for the view
            pet.PetImages = await _context.PetImages.Where(i => i.PetId == pet.Id).ToListAsync();
            return View(pet);
        }

        // POST: Admin/Pets/DeleteImage/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var image = await _context.PetImages.FindAsync(id);
            if (image == null) return NotFound();
            _context.PetImages.Remove(image);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Image deleted.";
            return RedirectToAction("Edit", new { id = image.PetId });
        }

        // GET: Admin/Pets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var pet = await _context.Pets.FirstOrDefaultAsync(m => m.Id == id);
            if (pet == null) return NotFound();

            return View(pet);
        }

        // POST: Admin/Pets/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Pets/DeleteConfirmed/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet == null)
                return NotFound();

            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();

            return Ok(); // No need to redirect since handled via JS
        }

        //Export to PDF
        public IActionResult ExportAdoptedToExcel()
        {
            var pets = _context.Pets.Where(p => p.Status == "Adopted").ToList();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("AdoptedPets");
            worksheet.Cell(1, 1).Value = "Name";
            worksheet.Cell(1, 2).Value = "Breed";
            worksheet.Cell(1, 3).Value = "Age";
            worksheet.Cell(1, 4).Value = "Gender";
            worksheet.Cell(1, 5).Value = "Description";

            for (int i = 0; i < pets.Count; i++)
            {
                worksheet.Cell(i + 2, 1).Value = pets[i].Name;
                worksheet.Cell(i + 2, 2).Value = pets[i].Breed;
                worksheet.Cell(i + 2, 3).Value = pets[i].Age;
                worksheet.Cell(i + 2, 4).Value = pets[i].Gender;
                worksheet.Cell(i + 2, 5).Value = pets[i].Description;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "AdoptedPets.xlsx");
        }

        public IActionResult ExportAdoptedToPdf()
        {
            var pets = _context.Pets.Where(p => p.Status == "Adopted").ToList();
            return new ViewAsPdf("Export/AdoptedPdf", pets)
            {
                FileName = "AdoptedPets.pdf"
            };
        }

        public IActionResult ExportAvailableToExcel()
        {
            var pets = _context.Pets.Where(p => p.Status == "Available").ToList();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("AvailablePets");
            worksheet.Cell(1, 1).Value = "Name";
            worksheet.Cell(1, 2).Value = "Breed";
            worksheet.Cell(1, 3).Value = "Age";
            worksheet.Cell(1, 4).Value = "Gender";
            worksheet.Cell(1, 5).Value = "Description";

            for (int i = 0; i < pets.Count; i++)
            {
                worksheet.Cell(i + 2, 1).Value = pets[i].Name;
                worksheet.Cell(i + 2, 2).Value = pets[i].Breed;
                worksheet.Cell(i + 2, 3).Value = pets[i].Age;
                worksheet.Cell(i + 2, 4).Value = pets[i].Gender;
                worksheet.Cell(i + 2, 5).Value = pets[i].Description;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "AvailablePets.xlsx");
        }

        public IActionResult ExportAvailableToPdf()
        {
            var pets = _context.Pets.Where(p => p.Status == "Available").ToList();
            return new ViewAsPdf("Export/AvailablePdf", pets)
            {
                FileName = "AvailablePets.pdf"
            };
        }
        // POST: Admin/Pets/BulkEdit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkEdit([FromBody] BulkEditRequest model)
        {
            if (model == null || model.PetIds == null || !model.PetIds.Any())
                return BadRequest("No pets selected.");

            var pets = await _context.Pets.Where(p => model.PetIds.Contains(p.Id)).ToListAsync();

            foreach (var pet in pets)
            {
                if (model.UpdateFields.PetType != null)
                    pet.PetType = model.UpdateFields.PetType;

                if (model.UpdateFields.Breed != null)
                    pet.Breed = model.UpdateFields.Breed;

                if (model.UpdateFields.Age.HasValue)
                    pet.Age = model.UpdateFields.Age.Value;

                if (model.UpdateFields.Description != null)
                    pet.Description = model.UpdateFields.Description;
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        public class BulkEditRequest
        {
            public List<int> PetIds { get; set; } = new();
            public UpdateFields UpdateFields { get; set; } = new();
        }

        public class UpdateFields
        {
            public string? PetType { get; set; }
            public string? Breed { get; set; }
            public int? Age { get; set; }
            public string? Description { get; set; }
        }

        //Bulk Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkDelete([FromBody] BulkDeleteRequest request)
        {
            if (request?.PetIds == null || !request.PetIds.Any())
                return BadRequest("No pets selected.");

            var petsToDelete = await _context.Pets
                .Where(p => request.PetIds.Contains(p.Id))
                .ToListAsync();

            _context.Pets.RemoveRange(petsToDelete);
            await _context.SaveChangesAsync();

            return Ok();
        }

        public class BulkDeleteRequest
        {
            public List<int> PetIds { get; set; } = new();
        }


    }

}
