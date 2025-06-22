using Microsoft.AspNetCore.Identity;

namespace Pawfectmatch_V._1.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<AdoptionApplication> ProcessedApplications { get; set; }
    }
}
