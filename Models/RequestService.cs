using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WorkshopsGov.Models.Common;

namespace WorkshopsGov.Models;

public class RequestService : AuditableEntityBase
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string Folio { get; set; } = string.Empty;

    [Required]
    public string ApplicationUserId { get; set; } = string.Empty;

    [ValidateNever]
    public ApplicationUser ApplicationUser { get; set; } = null!;

    [Required]
    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;

    [Required]
    public int DepartmentId { get; set; }
    public Department Department { get; set; } = null!;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateOnly ReceptionDate { get; set; }

    [Required]
    public bool Active { get; set; } = true;

    // Relaciones muchos a muchos con Inspections y Files
    public ICollection<Inspection> Inspections { get; set; } = new List<Inspection>();
    public ICollection<File> Files { get; set; } = new List<File>();
}