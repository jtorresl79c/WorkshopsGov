using System.ComponentModel.DataAnnotations;
using WorkshopsGov.Models.Common;

namespace WorkshopsGov.Models
{
    public class WorkshopQuoteStatus : AuditableEntityBase
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        public bool Active { get; set; } = true;
    }
}
