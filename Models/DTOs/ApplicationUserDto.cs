namespace WorkshopsGov.Models.DTOs;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ApplicationUserDto
{
    public string? Id { get; set; }
    [Display(Name = "Nombre")]
    [Required(ErrorMessage = "El campo Nombre es obligatorio.")]
    public string FirstName { get; set; } = string.Empty;
    [Display(Name = "Segundo nombre")]
    public string? SecondName { get; set; } = string.Empty;
    [Display(Name = "Apellido paterno")]
    [Required(ErrorMessage = "El campo Apellido paterno es obligatorio.")]
    public string PaternalLastName { get; set; } = string.Empty;
    [Display(Name = "Apellido materno")]
    public string? MaternalLastName { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? UserName { get; set; }
    [Required(ErrorMessage = "El campo Email es obligatorio.")]
    [EmailAddress(ErrorMessage = "Email inválido.")]
    public string? Email { get; set; }
    [Display(Name = "Número de telefono")]
    public string? PhoneNumber { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    [Display(Name = "Departamento")]
    [Required]
    public int? DepartmentId  { get; set; }
    [Display(Name = "Contraseña")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).{6,}$", 
        ErrorMessage = "La contraseña debe tener al menos una letra minúscula, una letra mayúscula, un número y un carácter especial.")]
    public string? Password { get; set; }
    [Display(Name = "Confirmar contraseña")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
    public string? ConfirmPassword { get; set; }
    [Display(Name = "Activo")]
    public bool Active { get; set; } = true;
}