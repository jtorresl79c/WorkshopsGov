using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkshopsGov.Models
{
    public class ExternalWorkshopBranch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
    
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)] // Ajusta según la longitud máxima esperada
        public string? Phone { get; set; } = string.Empty;

        [MaxLength(255)] // Ajusta según la longitud máxima esperada
        public string? Address { get; set; } = string.Empty;

        [Required]
        public int ExternalWorkshopId { get; set; }
        public ExternalWorkshop ExternalWorkshop { get; set; } = null!;

        [Required]
        public bool Active { get; set; } = true;
    }
}