namespace Pawfectmatch_V._1.Models
{
    public class AdoptionApplication
    {
        public int Id { get; set; }
        public string PetName { get; set; }
        public string Breed { get; set; }
        public string ApplicantName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Status { get; set; } // To Review / Approved / Declined
        public DateTime SubmittedAt { get; set; }
    }

}
