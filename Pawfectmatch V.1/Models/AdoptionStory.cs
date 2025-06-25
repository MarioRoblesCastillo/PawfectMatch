using System;
using System.ComponentModel.DataAnnotations;

namespace Pawfectmatch_V._1.Models
{
    public class AdoptionStory
    {
        public int Id { get; set; }

        [Required]
        public int ApplicationId { get; set; }
        public AdoptionApplication Application { get; set; }

        [Required, MaxLength(100)]
        public string Title { get; set; }

        [Required, MaxLength(2000)]
        public string StoryText { get; set; }

        public string PhotoPath { get; set; }

        public DateTime DatePosted { get; set; } = DateTime.Now;

        public bool IsApproved { get; set; } = true;
    }
} 