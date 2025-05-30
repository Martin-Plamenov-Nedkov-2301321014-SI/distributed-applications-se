using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCareMVC.Models
{
    public class Owner
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = null!;

        [NotMapped]
        public string FullName => FirstName + " " + LastName;

        [Required]
        [MaxLength(100)]
        public string Address { get; set; } = null!;

        [Required]
        [MaxLength(15)]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        public DateTime DateOfBirth { get; set; }

        public ICollection<Pet>? Pets { get; set; }
    }
}
