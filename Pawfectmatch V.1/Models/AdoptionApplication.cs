namespace Pawfectmatch_V._1.Models
{
    public class AdoptionApplication
    {
        public int Id { get; set; }

        // Relationship to Pet
        public int PetId { get; set; }
        public Pet Pet { get; set; }

        public string ApplicantName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        public string Status { get; set; } // e.g., To Review / Approved / Declined
        public DateTime SubmittedAt { get; set; }

        // Optional relationship to Admin (ApplicationUser)
        public string? AdminId { get; set; }
        public ApplicationUser? Admin { get; set; }
    }
}
