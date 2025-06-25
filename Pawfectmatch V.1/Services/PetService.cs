using Microsoft.EntityFrameworkCore;
using Pawfectmatch_V._1.Data;
using Pawfectmatch_V._1.Models;
using Pawfectmatch_V._1.Helpers;

namespace Pawfectmatch_V._1.Services
{
    public class PetService : IPetService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PetService(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<PaginatedList<Pet>> GetAvailablePetsAsync(string searchTerm, string petType, string gender, int page, int pageSize)
        {
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

            return await PaginatedList<Pet>.CreateAsync(query, page, pageSize);
        }

        public async Task<Pet?> GetPetByIdAsync(int id)
        {
            return await _context.Pets.FindAsync(id);
        }

        public async Task<List<Pet>> GetFeaturedPetsAsync(int count)
        {
            return await _context.Pets
                .Where(p => p.Status == "Available")
                .OrderByDescending(p => p.DatePosted)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Pet>> GetRelatedPetsAsync(int petId, int count)
        {
            var pet = await _context.Pets.FindAsync(petId);
            if (pet == null) return new List<Pet>();

            return await _context.Pets
                .Where(p => p.PetType == pet.PetType && 
                           p.Id != petId && 
                           p.Status == "Available")
                .Take(count)
                .ToListAsync();
        }

        public async Task<bool> CreatePetAsync(Pet pet, IFormFile? imageFile)
        {
            try
            {
                pet.DatePosted = DateTime.Now;
                pet.Name = CapitalizeWords(pet.Name);
                pet.Breed = CapitalizeWords(pet.Breed);
                pet.Description = CapitalizeWords(pet.Description);
                pet.Gender = CapitalizeWords(pet.Gender);
                pet.Status = CapitalizeWords(pet.Status);

                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploads = Path.Combine(_env.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploads);
                    var filePath = Path.Combine(uploads, imageFile.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    pet.ImagePath = "/uploads/" + imageFile.FileName;
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

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdatePetAsync(Pet pet, IFormFile? imageFile)
        {
            try
            {
                var existingPet = await _context.Pets.FindAsync(pet.Id);
                if (existingPet == null) return false;

                // Update fields manually
                existingPet.Name = CapitalizeWords(pet.Name);
                existingPet.PetType = CapitalizeWords(pet.PetType);
                existingPet.Breed = CapitalizeWords(pet.Breed);
                existingPet.Description = CapitalizeWords(pet.Description);
                existingPet.Gender = CapitalizeWords(pet.Gender);
                existingPet.Status = CapitalizeWords(pet.Status);
                existingPet.Age = pet.Age;

                if (existingPet.Status == "Adopted" && existingPet.DateOfRelease == null)
                {
                    existingPet.DateOfRelease = DateTime.Now;
                }

                // Handle image upload
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploads = Path.Combine(_env.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploads);
                    var filePath = Path.Combine(uploads, imageFile.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    existingPet.ImagePath = "/uploads/" + imageFile.FileName;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeletePetAsync(int id)
        {
            try
            {
                var pet = await _context.Pets.FindAsync(id);
                if (pet == null) return false;

                _context.Pets.Remove(pet);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdatePetStatusAsync(int petId, string status)
        {
            try
            {
                var pet = await _context.Pets.FindAsync(petId);
                if (pet == null) return false;

                pet.Status = status;
                if (status == "Adopted" && pet.DateOfRelease == null)
                {
                    pet.DateOfRelease = DateTime.Now;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<string>> GetPetTypesAsync()
        {
            return await _context.Pets
                .Where(p => p.Status == "Available")
                .Select(p => p.PetType)
                .Distinct()
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetPetStatisticsAsync()
        {
            var totalPets = await _context.Pets.CountAsync();
            var adoptedPets = await _context.Pets.CountAsync(p => p.Status == "Adopted");
            var availablePets = await _context.Pets.CountAsync(p => p.Status == "Available");
            var totalApplications = await _context.AdoptionApplications.CountAsync();

            return new Dictionary<string, int>
            {
                ["TotalPets"] = totalPets,
                ["AdoptedPets"] = adoptedPets,
                ["AvailablePets"] = availablePets,
                ["TotalApplications"] = totalApplications
            };
        }

        private string CapitalizeWords(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;

            var words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return string.Join(' ', words.Select(word =>
                char.ToUpper(word[0]) + word.Substring(1).ToLower()
            ));
        }
    }
} 