using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkshopsGov.Models
{
    public class VehicleStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)] // Ajusta según la longitud de tu varchar
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public bool Active { get; set; } = true;
    }
}