using System.ComponentModel.DataAnnotations;

namespace PetCareMVC.Models
{
    public class Pet
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; } = null!;

        [Required]
        public string Species { get; set; } = null!;

        [Required]
        public string Breed { get; set; } = null!;

        [Required]
        public DateTime DateOfBirth { get; set; }

        public double WeightKg { get; set; }

        public int OwnerId { get; set; }
        public Owner? Owner { get; set; }

        public ICollection<HealthRecord>? HealthRecords { get; set; }

        [MaxLength(500)]
        public string? PhotoPath { get; set; }
    }
}
