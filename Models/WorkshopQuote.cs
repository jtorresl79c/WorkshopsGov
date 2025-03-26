using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkshopsGov.Models
{
    public class WorkshopQuote
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int InspectionId { get; set; }

        [ForeignKey(nameof(InspectionId))]
        public Inspection Inspection { get; set; } = null!;

        [Required]
        public int WorkshopBranchId { get; set; }

        [ForeignKey(nameof(WorkshopBranchId))]
        public ExternalWorkshopBranch WorkshopBranch { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string QuoteNumber { get; set; } = null!;

        [Required]
        public DateTime QuoteDate { get; set; }

        [Required]
        public decimal TotalCost { get; set; }

        [Required]
        public DateTime EstimatedCompletionDate { get; set; }

        [ForeignKey("QuoteStatus")]
        public int QuoteStatusId { get; set; }
        public WorkshopQuoteStatus QuoteStatus { get; set; } = null!;

        public string QuoteDetails { get; set; } = string.Empty;

        public bool Active { get; set; } = true;
        public ICollection<File> Files { get; set; } = new List<File>();

    }
}
