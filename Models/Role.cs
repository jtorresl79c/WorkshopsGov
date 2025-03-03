namespace WorkshopsGov.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Role
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    
    [Required] // Campo obligatorio
    public bool Active { get; set; } = true; // Activo por defecto

    // Relación con los usuarios
    public ICollection<ApplicationUser>? Users { get; set; }
}