using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WorkshopsGov.Models.Common;

namespace WorkshopsGov.Models
{
    public class Diagnostic : AuditableEntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string MemoNumber { get; set; } = string.Empty;

        [Required]
        public DateTime DiagnosticDate { get; set; }

        [Required]
        public TimeSpan CheckInTime { get; set; }

        [Required]
        [MaxLength(255)]
        public string OperatorName { get; set; } = string.Empty;

        // Relación con ApplicationUser (Usuario que hizo el diagnóstico)
        [Required]
        public string ApplicationUserId { get; set; } = string.Empty;

        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser ApplicationUser { get; set; } = null!;

        // Relación con otros modelos
        [Required]
        public int DiagnosticServiceStatusId { get; set; }
        public DiagnosticServiceStatus DiagnosticServiceStatus { get; set; } = null!;

        [Required]
        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = null!;

        [Required]
        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;

        [Required]
        public int ExternalWorkshopBranchId { get; set; }
        public ExternalWorkshopBranch ExternalWorkshopBranch { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string DistanceUnit { get; set; } = string.Empty;

        [Required]
        public float DistanceValue { get; set; }

        [Required]
        public float FuelLevel { get; set; }

        [Required]
        public string FailureReport { get; set; } = string.Empty;

        [Required]
        public string VehicleFailureObservation { get; set; } = string.Empty;

        [Required]
        public string Repairs { get; set; } = string.Empty;

        [Required]
        public int DiagnosticStatusId { get; set; }
        public DiagnosticStatus DiagnosticStatus { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string MechanicName { get; set; } = string.Empty;

        [Required]
        public int DiagnosticPartId { get; set; }
        public DiagnosticPart DiagnosticPart { get; set; } = null!;

        public DateTime? CompletionDate { get; set; }

        // Relación muchos a muchos con VehicleFailure
        public ICollection<VehicleFailure> VehicleFailures { get; set; } = new List<VehicleFailure>();
    }
}