using System.ComponentModel.DataAnnotations;

namespace WorkshopsGov.Models.DTOs
{
    public class UserLoguinRequestDto
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
