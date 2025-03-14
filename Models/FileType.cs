using System.ComponentModel.DataAnnotations;
using WorkshopsGov.Models.Common;

namespace WorkshopsGov.Models;

public class FileType : AuditableEntityBase
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public bool Active { get; set; } = true;
}
