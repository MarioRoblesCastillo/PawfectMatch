using Microsoft.EntityFrameworkCore;
using Pawfectmatch_V._1.Data;
using Pawfectmatch_V._1.Models;
using Pawfectmatch_V._1.Helpers;

namespace Pawfectmatch_V._1.Repositories
{
    public class PetRepository : Repository<Pet>, IPetRepository
    {
        public PetRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PaginatedList<Pet>> GetAvailablePetsAsync(int page, int pageSize)
        {
            var query = _dbSet.Where(p => p.Status == "Available")
                              .OrderByDescending(p => p.DatePosted);

            return await PaginatedList<Pet>.CreateAsync(query, page, pageSize);
        }

        public async Task<IEnumerable<Pet>> GetPetsByStatusAsync(string status)
        {
            return await _dbSet.Where(p => p.Status == status)
                               .OrderByDescending(p => p.DatePosted)
                               .ToListAsync();
        }

        public async Task<IEnumerable<Pet>> GetPetsByTypeAsync(string petType)
        {
            return await _dbSet.Where(p => p.PetType == petType && p.Status == "Available")
                               .OrderByDescending(p => p.DatePosted)
                               .ToListAsync();
        }

        public async Task<IEnumerable<Pet>> SearchPetsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<Pet>();

            return await _dbSet.Where(p => p.Status == "Available" &&
                                          (p.Name.Contains(searchTerm) || 
                                           p.Breed.Contains(searchTerm) || 
                                           p.Description.Contains(searchTerm)))
                               .OrderByDescending(p => p.DatePosted)
                               .ToListAsync();
        }

        public async Task<int> GetPetCountByStatusAsync(string status)
        {
            return await _dbSet.CountAsync(p => p.Status == status);
        }

        public async Task<IEnumerable<Pet>> GetRecentlyAddedPetsAsync(int count)
        {
            return await _dbSet.Where(p => p.Status == "Available")
                               .OrderByDescending(p => p.DatePosted)
                               .Take(count)
                               .ToListAsync();
        }

        public async Task<IEnumerable<Pet>> GetAdoptedPetsAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _dbSet.Where(p => p.Status == "Adopted");

            if (fromDate.HasValue)
                query = query.Where(p => p.DateOfRelease >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(p => p.DateOfRelease <= toDate.Value);

            return await query.OrderByDescending(p => p.DateOfRelease)
                              .ToListAsync();
        }
    }
} 