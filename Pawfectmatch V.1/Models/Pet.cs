using System.ComponentModel.DataAnnotations;

namespace Pawfectmatch_V._1.Models
{
    public class Pet
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public required string Name { get; set; }

        [Required, StringLength(100)]
        public required string Breed { get; set; }

        [Range(0, 100)]
        public int Age { get; set; }

        [Required]
        public required string Gender { get; set; }

        [Required(ErrorMessage = "Pet type is required.")]
        public required string PetType { get; set; }  // e.g., "Dog" or "Cat"


        [StringLength(500)]
        public string? Description { get; set; }

        public string? ImagePath { get; set; }

        [Required]
        public required string Status { get; set; } 

        public DateTime? DateOfRelease { get; set; }  

        public string? MicrochipNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime DatePosted { get; set; }

        public ICollection<AdoptionApplication> AdoptionApplications { get; set; } = new List<AdoptionApplication>();

    }
}
