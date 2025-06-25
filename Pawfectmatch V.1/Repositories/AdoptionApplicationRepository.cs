using Microsoft.EntityFrameworkCore;
using Pawfectmatch_V._1.Data;
using Pawfectmatch_V._1.Models;

namespace Pawfectmatch_V._1.Repositories
{
    public class AdoptionApplicationRepository : Repository<AdoptionApplication>, IAdoptionApplicationRepository
    {
        public AdoptionApplicationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<AdoptionApplication>> GetApplicationsByStatusAsync(string status)
        {
            return await _dbSet.Where(a => a.Status == status)
                               .Include(a => a.Pet)
                               .Include(a => a.Admin)
                               .OrderByDescending(a => a.SubmittedAt)
                               .ToListAsync();
        }

        public async Task<IEnumerable<AdoptionApplication>> GetApplicationsByPetAsync(int petId)
        {
            return await _dbSet.Where(a => a.PetId == petId)
                               .Include(a => a.Admin)
                               .OrderByDescending(a => a.SubmittedAt)
                               .ToListAsync();
        }

        public async Task<IEnumerable<AdoptionApplication>> GetApplicationsByAdminAsync(string adminId)
        {
            return await _dbSet.Where(a => a.AdminId == adminId)
                               .Include(a => a.Pet)
                               .OrderByDescending(a => a.SubmittedAt)
                               .ToListAsync();
        }

        public async Task<int> GetApplicationCountByStatusAsync(string status)
        {
            return await _dbSet.CountAsync(a => a.Status == status);
        }

        public async Task<IEnumerable<AdoptionApplication>> GetRecentApplicationsAsync(int count)
        {
            return await _dbSet.Include(a => a.Pet)
                               .Include(a => a.Admin)
                               .OrderByDescending(a => a.SubmittedAt)
                               .Take(count)
                               .ToListAsync();
        }

        public async Task<IEnumerable<AdoptionApplication>> GetApplicationsWithDetailsAsync()
        {
            return await _dbSet.Include(a => a.Pet)
                               .Include(a => a.Admin)
                               .OrderByDescending(a => a.SubmittedAt)
                               .ToListAsync();
        }
    }
} 