namespace Pawfectmatch_V._1.Models
{
    public class AdoptionApplication
    {
        public int Id { get; set; }
        
        // Applicant Info
        public string ApplicantName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        // Application Info
        public DateTime SubmittedAt { get; set; }
        public string ReqStatus { get; set; } // Pending / Approved / Rejected

        // Foreign Key to Pet
        public int PetId { get; set; }
        public Pet Pet { get; set; }

        // Foreign Key to Admin
        public string? AdminId { get; set; }  // If using ASP.NET Identity
        public ApplicationUser? Admin { get; set; }
    }

}
