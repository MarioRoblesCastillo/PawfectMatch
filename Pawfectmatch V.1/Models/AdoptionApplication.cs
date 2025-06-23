namespace Pawfectmatch_V._1.Models
{
    public class AdoptionApplication
    {
        public int Id { get; set; }
        
        // Applicant Info
        public required string ApplicantName { get; set; }
        public required string Email { get; set; }
        public required string Address { get; set; }

        // Application Info
        public DateTime SubmittedAt { get; set; }
        public required string ReqStatus { get; set; } // Pending / Approved / Rejected

        // Foreign Key to Pet
        public int PetId { get; set; }
        public required Pet Pet { get; set; }

        // Foreign Key to Admin
        public string? AdminId { get; set; }  // using ASP.NET Identity
        public ApplicationUser? Admin { get; set; }
    }

}
