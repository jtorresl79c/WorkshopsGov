using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WorkshopsGov.Models.Common;

namespace WorkshopsGov.Models
{
    public class Sector : AuditableEntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)] // Ajusta según la longitud de tu varchar
        public string Name { get; set; } = string.Empty;

        public bool Active { get; set; } = true;
        
        public ICollection<Department> Departments { get; set; } = new List<Department>();
    }
}