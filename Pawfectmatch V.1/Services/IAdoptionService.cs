using Pawfectmatch_V._1.Models;

namespace Pawfectmatch_V._1.Services
{
    public interface IAdoptionService
    {
        Task<List<AdoptionApplication>> GetApplicationsAsync(string status = "All");
        Task<AdoptionApplication?> GetApplicationByIdAsync(int id);
        Task<bool> CreateApplicationAsync(AdoptionApplication application);
        Task<bool> UpdateApplicationStatusAsync(int applicationId, string status, string adminId);
        Task<bool> DeleteApplicationAsync(int id);
        Task<Dictionary<string, int>> GetApplicationStatisticsAsync();
        Task<List<AdoptionApplication>> GetApplicationsByPetAsync(int petId);
        Task<bool> ValidateApplicationAsync(AdoptionApplication application);
    }
} 