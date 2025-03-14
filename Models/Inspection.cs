using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WorkshopsGov.Models.Common;

namespace WorkshopsGov.Models
{
    public class Inspection : AuditableEntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string MemoNumber { get; set; } = string.Empty;

        [Required]
        public DateTime InspectionDate { get; set; }

        [Required]
        public TimeSpan CheckInTime { get; set; }

        [Required]
        [MaxLength(255)]
        public string OperatorName { get; set; } = string.Empty;

        // Relación con ApplicationUser (Usuario que hizo la inspeccion)
        [Required]
        public string ApplicationUserId { get; set; } = string.Empty;

        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser ApplicationUser { get; set; } = null!;
        
        // Relación con otros modelos
        [Required]
        public int InspectionServiceId { get; set; }
        public InspectionService InspectionService { get; set; } = null!;
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
        public bool TowRequired { get; set; } = false;

        [Required]
        public int InspectionStatusId { get; set; }
        public InspectionStatus InspectionStatus { get; set; } = null!;

        [Required]
        public string Diagnostic { get; set; } = string.Empty;
        
        [Required]
        public bool Active { get; set; } = true;

        // Relación muchos a muchos con VehicleFailure
        public ICollection<VehicleFailure> VehicleFailures { get; set; } = new List<VehicleFailure>();
        // 🔹 Relación muchos a muchos con File
        public ICollection<File> Files { get; set; } = new List<File>();

    }
}