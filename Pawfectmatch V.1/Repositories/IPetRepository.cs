using Pawfectmatch_V._1.Models;
using Pawfectmatch_V._1.Helpers;

namespace Pawfectmatch_V._1.Repositories
{
    public interface IPetRepository : IRepository<Pet>
    {
        Task<PaginatedList<Pet>> GetAvailablePetsAsync(int page, int pageSize);
        Task<IEnumerable<Pet>> GetPetsByStatusAsync(string status);
        Task<IEnumerable<Pet>> GetPetsByTypeAsync(string petType);
        Task<IEnumerable<Pet>> SearchPetsAsync(string searchTerm);
        Task<int> GetPetCountByStatusAsync(string status);
        Task<IEnumerable<Pet>> GetRecentlyAddedPetsAsync(int count);
        Task<IEnumerable<Pet>> GetAdoptedPetsAsync(DateTime? fromDate = null, DateTime? toDate = null);
    }
} 