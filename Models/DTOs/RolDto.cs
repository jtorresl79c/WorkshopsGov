namespace WorkshopsGov.Models.DTOs;
using System.ComponentModel.DataAnnotations;

public class RolDto
{
    [Required]
    public string Id { get; set; }
    [Required]
    public string Name { get; set; } = String.Empty;
    [Required]
    public string NormalizedName { get; set; }= String.Empty;
}