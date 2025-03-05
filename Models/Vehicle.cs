using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WorkshopsGov.Models.Common;

namespace WorkshopsGov.Models;

public class Vehicle : AuditableEntityBase
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Oficialia { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string LicensePlate { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string VinNumber { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public int DepartmentId { get; set; }
    public Department Department { get; set; } = null!;

    [Required]
    public int VehicleStatusId { get; set; }
    public VehicleStatus VehicleStatus { get; set; } = null!;

    public int? Year { get; set; }

    [Required]
    public int BrandId { get; set; }
    public Brand Brand { get; set; } = null!;

    [Required]
    public int ModelId { get; set; }
    public VehicleModel Model { get; set; } = null!;

    [Required]
    [MaxLength(255)]
    public string Engine { get; set; } = string.Empty;

    [Required]
    public int SectorId { get; set; }
    public Sector Sector { get; set; } = null!;

    [Required]
    public int VehicleTypeId { get; set; }
    public VehicleType VehicleType { get; set; } = null!;

    [Required]
    public bool Active { get; set; } = true;
}