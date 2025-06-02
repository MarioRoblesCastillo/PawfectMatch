using System.ComponentModel.DataAnnotations;

namespace Pawfectmatch_V._1.Models
{
    public class Pet
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required, StringLength(100)]
        public string Breed { get; set; }

        [Range(0, 100)]
        public int Age { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Pet type is required.")]
        public string PetType { get; set; }  // e.g., "Dog" or "Cat"


        [StringLength(500)]
        public string? Description { get; set; }

        public string? ImagePath { get; set; }

        [Required]
        public string Status { get; set; } 

        public DateTime? DateOfRelease { get; set; }  

        public string? MicrochipNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime DatePosted { get; set; }

    }
}
