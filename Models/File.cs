using System.ComponentModel.DataAnnotations;
using WorkshopsGov.Models.Common;
namespace WorkshopsGov.Models;

public class File : AuditableEntityBase
{
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Format { get; set; } = string.Empty;

    public float Size { get; set; }

    [Required]
    public string Path { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int FileTypeId { get; set; }
    public FileType FileType { get; set; } = null!;

    [Required]
    public string ApplicationUserId { get; set; } = string.Empty;
    public ApplicationUser ApplicationUser { get; set; } = null!;

    [Required]
    public bool Active { get; set; } = true;
    // Relación muchos a muchos con Inspection
    public ICollection<Inspection> Inspections { get; set; } = new List<Inspection>();
}

