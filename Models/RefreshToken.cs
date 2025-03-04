using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WorkshopsGov.Models.Common;

namespace WorkshopsGov.Models
{
    public class RefreshToken : AuditableEntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string ApplicationUserId { get; set; } = string.Empty; // Renombrado para mayor claridad

        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser ApplicationUser { get; set; } = null!;

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public string JwtId { get; set; } = string.Empty;

        [Required]
        public bool IsUsed { get; set; } = false;

        [Required]
        public bool IsRevoked { get; set; } = false;

        [Required]
        public DateTime AddedDate { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }
    }
}