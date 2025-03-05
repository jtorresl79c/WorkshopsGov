namespace WorkshopsGov.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WorkshopsGov.Models.Common;

public class Department : AuditableEntityBase
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public int SectorId { get; set; }
    public Sector Sector { get; set; } = null!;
    
    [Required]
    public bool Active { get; set; } = true;

    // Relación con los usuarios
    public ICollection<ApplicationUser>? Users { get; set; }
    public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
