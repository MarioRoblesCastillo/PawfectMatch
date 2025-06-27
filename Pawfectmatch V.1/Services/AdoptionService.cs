using Microsoft.EntityFrameworkCore;
using Pawfectmatch_V._1.Data;
using Pawfectmatch_V._1.Models;

namespace Pawfectmatch_V._1.Services
{
    public class AdoptionService : IAdoptionService
    {
        private readonly ApplicationDbContext _context;

        public AdoptionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AdoptionApplication>> GetApplicationsAsync(string status = "All")
        {
            var query = _context.AdoptionApplications
                .Include(a => a.Pet)
                .Include(a => a.Admin)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                query = query.Where(a => a.Status == status);
            }

            return await query
                .OrderByDescending(a => a.SubmittedAt)
                .ToListAsync();
        }

        public async Task<AdoptionApplication?> GetApplicationByIdAsync(int id)
        {
            return await _context.AdoptionApplications
                .Include(a => a.Pet)
                .Include(a => a.Admin)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<bool> CreateApplicationAsync(AdoptionApplication application)
        {
            try
            {
                // Validate the application
                if (!await ValidateApplicationAsync(application))
                {
                    return false;
                }

                // Set default values
                application.Status = "To Review";
                application.SubmittedAt = DateTime.Now;

                _context.AdoptionApplications.Add(application);
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateApplicationStatusAsync(int applicationId, string status, string adminId)
        {
            try
            {
                var application = await _context.AdoptionApplications
                    .Include(a => a.Pet)
                    .FirstOrDefaultAsync(a => a.Id == applicationId);

                if (application == null)
                    return false;

                // Update application status
                application.Status = status;
                application.AdminId = adminId;

                // If approved, update pet status to adopted
                if (status == "Approved" && application.Pet != null)
                {
                    application.Pet.Status = "Adopted";
                    application.Pet.DateOfRelease = DateTime.Now;
                }

                // If declined, make sure pet is available again
                if (status == "Declined" && application.Pet != null && application.Pet.Status == "Adopted")
                {
                    application.Pet.Status = "Available";
                    application.Pet.DateOfRelease = null;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteApplicationAsync(int id)
        {
            try
            {
                var application = await _context.AdoptionApplications.FindAsync(id);
                if (application == null)
                    return false;

                _context.AdoptionApplications.Remove(application);
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Dictionary<string, int>> GetApplicationStatisticsAsync()
        {
            var totalApplications = await _context.AdoptionApplications.CountAsync();
            var pendingApplications = await _context.AdoptionApplications.CountAsync(a => a.Status == "To Review");
            var approvedApplications = await _context.AdoptionApplications.CountAsync(a => a.Status == "Approved");
            var declinedApplications = await _context.AdoptionApplications.CountAsync(a => a.Status == "Declined");

            return new Dictionary<string, int>
            {
                ["TotalApplications"] = totalApplications,
                ["PendingApplications"] = pendingApplications,
                ["ApprovedApplications"] = approvedApplications,
                ["DeclinedApplications"] = declinedApplications
            };
        }

        public async Task<List<AdoptionApplication>> GetApplicationsByPetAsync(int petId)
        {
            return await _context.AdoptionApplications
                .Include(a => a.Admin)
                .Where(a => a.PetId == petId)
                .OrderByDescending(a => a.SubmittedAt)
                .ToListAsync();
        }

        public async Task<bool> ValidateApplicationAsync(AdoptionApplication application)
        {
            // Check if pet exists and is available
            var pet = await _context.Pets.FindAsync(application.PetId);
            if (pet == null || pet.Status != "Available")
            {
                return false;
            }

            // Basic validation for required fields
            if (string.IsNullOrWhiteSpace(application.ApplicantName) ||
                string.IsNullOrWhiteSpace(application.Email) ||
                string.IsNullOrWhiteSpace(application.Address))
            {
                return false;
            }

            // Check if email is valid format (basic check)
            if (!application.Email.Contains("@"))
            {
                return false;
            }

            return true;
        }
    }
} 