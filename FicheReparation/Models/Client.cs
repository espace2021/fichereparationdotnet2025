using System.ComponentModel.DataAnnotations;

namespace FicheReparation.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        public string? Adresse { get; set; }

        [Required]
        public string Nom { get; set; }

        [Required]
        [Phone]
        public string NumTel { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
