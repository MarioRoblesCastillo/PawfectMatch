using Pawfectmatch_V._1.Models;

namespace Pawfectmatch_V._1.Repositories
{
    public interface IAdoptionApplicationRepository : IRepository<AdoptionApplication>
    {
        Task<IEnumerable<AdoptionApplication>> GetApplicationsByStatusAsync(string status);
        Task<IEnumerable<AdoptionApplication>> GetApplicationsByPetAsync(int petId);
        Task<IEnumerable<AdoptionApplication>> GetApplicationsByAdminAsync(string adminId);
        Task<int> GetApplicationCountByStatusAsync(string status);
        Task<IEnumerable<AdoptionApplication>> GetRecentApplicationsAsync(int count);
        Task<IEnumerable<AdoptionApplication>> GetApplicationsWithDetailsAsync();
    }
} 