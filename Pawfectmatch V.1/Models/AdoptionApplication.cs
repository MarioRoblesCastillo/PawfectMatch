namespace Pawfectmatch_V._1.Models
{
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public class AdoptionApplication
    {
        public int Id { get; set; }

        // Relationship to Pet
        [Required]
        public int PetId { get; set; }
        [BindNever]
        public Pet? Pet { get; set; }

        public string ApplicantName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        public string Status { get; set; } // e.g., To Review / Approved / Declined
        public DateTime SubmittedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        // Optional relationship to Admin (ApplicationUser)
        public string? AdminId { get; set; }
        public ApplicationUser? Admin { get; set; }

        public string? Phone { get; set; }
        public string? ResidenceType { get; set; }
        public string? OwnOrRent { get; set; }
        public string? LandlordAllowsPets { get; set; }
        public string? SecureYard { get; set; }
        public string? CurrentlyHavePets { get; set; }
        public string? OwnedPetsBefore { get; set; }
        public string? PetCareWhenAway { get; set; }
        public string? FinanciallyPrepared { get; set; }
        public string? AllowHomeVisit { get; set; }
        public string? LongTermCommitment { get; set; }
        public string? Signature { get; set; }
        public DateTime? Date { get; set; }
    }
}
