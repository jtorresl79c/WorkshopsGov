using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkshopsGov.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    [MaxLength(255)]
    public string FirstName { get; set; } = string.Empty;
    [MaxLength(255)]
    public string SecondName { get; set; } = string.Empty;
    [Required]
    [MaxLength(255)]
    public string PaternalLastName { get; set; } = string.Empty;
    [MaxLength(255)]
    public string MaternalLastName { get; set; } = string.Empty;
    public int? DepartmentId { get; set; }  // Clave foránea
    public Department? Department { get; set; }  // Propiedad de navegación
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    [Required]
    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}