using System.ComponentModel.DataAnnotations;

namespace PetCareMVC.Models
{
    public class HealthRecord
    {
        public int Id { get; set; }

        [Required]
        public DateTime VisitDate { get; set; }

        [Required]
        [MaxLength(100)]
        public string Description { get; set; } = null!;

        public double Temperature { get; set; }

        public bool IsVaccinated { get; set; }

        public long MicrochipNumber { get; set; }

        public int PetId { get; set; }
        public Pet? Pet { get; set; }
    }
}
