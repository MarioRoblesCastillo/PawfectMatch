using Pawfectmatch_V._1.Models;
using Pawfectmatch_V._1.Helpers;

namespace Pawfectmatch_V._1.Services
{
    public interface IPetService
    {
        Task<PaginatedList<Pet>> GetAvailablePetsAsync(string searchTerm, string petType, string gender, int page, int pageSize);
        Task<Pet?> GetPetByIdAsync(int id);
        Task<List<Pet>> GetFeaturedPetsAsync(int count);
        Task<List<Pet>> GetRelatedPetsAsync(int petId, int count);
        Task<bool> CreatePetAsync(Pet pet, IFormFile? imageFile);
        Task<bool> UpdatePetAsync(Pet pet, IFormFile? imageFile);
        Task<bool> DeletePetAsync(int id);
        Task<bool> UpdatePetStatusAsync(int petId, string status);
        Task<List<string>> GetPetTypesAsync();
        Task<Dictionary<string, int>> GetPetStatisticsAsync();
    }
} 