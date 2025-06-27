using System.ComponentModel.DataAnnotations;

namespace Pawfectmatch_V._1.Models
{
    public class PetImage
    {
        public int Id { get; set; }
        [Required]
        public int PetId { get; set; }
        public Pet Pet { get; set; }
        [Required]
        public string ImagePath { get; set; }
        public int SortOrder { get; set; } = 0;
    }
} 