using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FicheReparation.Models
{
    
    public class DemandeReparation
    {
        [Key]
         public int Id { get; set; }

        [Required]
        public DateTime DateDepotAppareil { get; set; }

        [Required]
        [StringLength(100)]
        public string Appareil { get; set; }

        [Required]
        [StringLength(50)]
        public string Etat { get; set; }

        [Required]
        [StringLength(255)]
        public string SymptomesPanne { get; set; }

        // Clé étrangère vers Client
        [Required]
        public int ClientId { get; set; }

        [ForeignKey("ClientId")]
        public Client? Client { get; set; }
    }

}

