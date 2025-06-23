using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Pawfectmatch_V._1.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<AdoptionApplication> ProcessedApplications { get; set; } = new List<AdoptionApplication>();
    }
}
