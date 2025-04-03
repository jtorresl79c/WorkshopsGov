using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorkshopsGov.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using WorkshopsGov.Models.Common;
using File = WorkshopsGov.Models.File;

namespace WorkshopsGov.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Sector> Sectors { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<ExternalWorkshop> ExternalWorkshops { get; set; }
    public DbSet<ExternalWorkshopBranch> ExternalWorkshopBranches { get; set; }
    public DbSet<VehicleStatus> VehicleStatuses { get; set; }
    public DbSet<VehicleType> VehicleTypes { get; set; }
    public DbSet<VehicleFailure> VehicleFailures { get; set; }
    public DbSet<InspectionService> InspectionServices { get; set; }
    public DbSet<InspectionStatus> InspectionStatuses { get; set; }
    public DbSet<VehicleModel> VehicleModels { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<InspectionPart> InspectionParts { get; set; }
    public DbSet<Inspection> Inspections { get; set; }
    public DbSet<FileType> FileTypes { get; set; }
    public DbSet<File> Files { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<WorkshopQuote> WorkshopQuote { get; set; }
    public DbSet<WorkshopQuoteStatus> WorkshopQuoteStatus { get; set; }
    public DbSet<RequestService> RequestServices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes()) // <-- Asegurar que el tipo es IMutableEntityType
        {
            // Convertir nombres de tablas a snake_case
            var tableName = entity.GetTableName();
            if (tableName != null)
            {
                entity.SetTableName(ToSnakeCase(tableName));
            }

            // Convertir nombres de columnas a snake_case
            foreach (IMutableProperty property in entity.GetProperties()) // <-- Asegurar que el tipo es IMutableProperty
            {
                var columnName = property.GetColumnName();
                if (columnName != null)
                {
                    property.SetColumnName(ToSnakeCase(columnName));
                }
            }
        }
        
        modelBuilder.Entity<ApplicationUser>()
            .Property(e => e.FirstName)
            .HasMaxLength(250);
        modelBuilder.Entity<ApplicationUser>()
            .Property(e => e.SecondName)
            .HasDefaultValue(string.Empty)
            .HasMaxLength(250);
        modelBuilder.Entity<ApplicationUser>()
            .Property(e => e.PaternalLastName)
            .HasDefaultValue(string.Empty)
            .HasMaxLength(250);
        modelBuilder.Entity<ApplicationUser>()
            .Property(e => e.MaternalLastName)
            .HasDefaultValue(string.Empty)
            .HasMaxLength(250);

        modelBuilder.Entity<ApplicationUser>()
            .HasOne(u => u.Department)
            .WithMany(d => d.Users)
            .HasForeignKey(u => u.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict); // O usa DeleteBehavior.Cascade si quieres que al eliminar un departamento se borren los usuarios asociados

        modelBuilder.Entity<ApplicationUser>()
            .HasMany(u => u.ExternalWorkshops)
            .WithMany(w => w.Users)
            .UsingEntity<Dictionary<string, object>>(
                "external_workshop_user",
                j => j
                    .HasOne<ExternalWorkshop>()
                    .WithMany()
                    .HasForeignKey("ExternalWorkshopId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
            );

        modelBuilder.Entity<Brand>()
            .Property(b => b.Active)
            .HasDefaultValue(true); // Valor por defecto a nivel de BD
        
        modelBuilder.Entity<Sector>()
            .Property(s => s.Active)
            .HasDefaultValue(true); // Valor por defecto a nivel de BD
        
        modelBuilder.Entity<ExternalWorkshop>()
            .Property(e => e.Active)
            .HasDefaultValue(true); // Valor por defecto a nivel de BD
        
        modelBuilder.Entity<VehicleStatus>()
            .Property(v => v.Active)
            .HasDefaultValue(true); // Valor por defecto a nivel de BD
        
        modelBuilder.Entity<VehicleType>()
            .Property(v => v.Active)
            .HasDefaultValue(true); // Valor por defecto a nivel de BD
        
        modelBuilder.Entity<VehicleFailure>()
            .Property(vf => vf.Active)
            .HasDefaultValue(true); // Valor por defecto en la BD
        
        modelBuilder.Entity<InspectionService>()
            .Property(dss => dss.Active)
            .HasDefaultValue(true); // Valor por defecto en la BD
        
        modelBuilder.Entity<InspectionStatus>()
            .Property(ds => ds.Active)
            .HasDefaultValue(true); // Valor por defecto en la BD
        
        modelBuilder.Entity<VehicleModel>()
            .HasOne(vm => vm.Brand)
            .WithMany(b => b.VehicleModels)
            .HasForeignKey(vm => vm.BrandId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Department>()
            .HasOne(d => d.Sector)
            .WithMany(s => s.Departments)
            .HasForeignKey(d => d.SectorId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<ExternalWorkshopBranch>()
            .HasOne(ewb => ewb.ExternalWorkshop)
            .WithMany(ew => ew.ExternalWorkshopBranches)
            .HasForeignKey(ewb => ewb.ExternalWorkshopId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.ApplicationUser)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade); // Cambia a Restrict si no quieres eliminación en cascada
        
        modelBuilder.Entity<ApplicationUser>()
            .Property(u => u.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<ApplicationUser>()
            .Property(u => u.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        
        modelBuilder.Entity<ApplicationUser>()
            .Property(u => u.Active)
            .HasDefaultValue(true) // Valor por defecto en BD
            .IsRequired(); // Asegura que no sea NULL
        
        // Nuevo
        modelBuilder.Entity<Vehicle>()
        .Property(v => v.Oficialia)
        .HasDefaultValue(string.Empty)
        .IsRequired();

        modelBuilder.Entity<Vehicle>()
            .Property(v => v.LicensePlate)
            .HasDefaultValue(string.Empty)
            .IsRequired();

        modelBuilder.Entity<Vehicle>()
            .Property(v => v.VinNumber)
            .HasDefaultValue(string.Empty)
            .IsRequired();

        modelBuilder.Entity<Vehicle>()
            .Property(v => v.Description)
            .HasDefaultValue(string.Empty)
            .IsRequired();

        modelBuilder.Entity<Vehicle>()
            .Property(v => v.Engine)
            .HasDefaultValue(string.Empty)
            .IsRequired();

        modelBuilder.Entity<Vehicle>()
            .Property(v => v.Active)
            .HasDefaultValue(true)
            .IsRequired();

        modelBuilder.Entity<Vehicle>()
            .HasIndex(v => v.LicensePlate)
            .IsUnique();

        modelBuilder.Entity<Vehicle>()
            .HasIndex(v => v.VinNumber)
            .IsUnique();

        modelBuilder.Entity<Vehicle>()
            .HasOne(v => v.Department)
            .WithMany(d => d.Vehicles)
            .HasForeignKey(v => v.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Vehicle>()
            .HasOne(v => v.VehicleStatus)
            .WithMany()
            .HasForeignKey(v => v.VehicleStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Vehicle>()
            .HasOne(v => v.Brand)
            .WithMany()
            .HasForeignKey(v => v.BrandId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Vehicle>()
            .HasOne(v => v.Model)
            .WithMany()
            .HasForeignKey(v => v.ModelId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Vehicle>()
            .HasOne(v => v.Sector)
            .WithMany()
            .HasForeignKey(v => v.SectorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Vehicle>()
            .HasOne(v => v.VehicleType)
            .WithMany()
            .HasForeignKey(v => v.VehicleTypeId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<InspectionPart>()
            .Property(ip => ip.Active)
            .IsRequired()
            .HasDefaultValue(true);

        modelBuilder.Entity<InspectionPart>()
            .Property(ip => ip.Name)
            .IsRequired()
            .HasMaxLength(255);
        
        // Relación muchos a muchos entre Inspection y VehicleFailure sin modelo intermedio
        modelBuilder.Entity<Inspection>()
            .HasMany(i => i.VehicleFailures)
            .WithMany(vf => vf.Inspections)
            .UsingEntity<Dictionary<string, object>>(
                "InspectionVehicleFailure", // Nombre de la tabla intermedia
                j => j.HasOne<VehicleFailure>()
                    .WithMany()
                    .HasForeignKey("VehicleFailureId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Inspection>()
                    .WithMany()
                    .HasForeignKey("InspectionId")
                    .OnDelete(DeleteBehavior.Cascade)
            );
        
        // Asegurar que los campos Required sean NOT NULL en la BD
        modelBuilder.Entity<Inspection>()
            .Property(i => i.MemoNumber)
            .IsRequired();

        modelBuilder.Entity<Inspection>()
            .Property(i => i.InspectionDate)
            .IsRequired();

        modelBuilder.Entity<Inspection>()
            .Property(i => i.CheckInTime)
            .IsRequired();

        modelBuilder.Entity<Inspection>()
            .Property(i => i.OperatorName)
            .HasMaxLength(255)
            .IsRequired();

        modelBuilder.Entity<Inspection>()
            .Property(i => i.DistanceUnit)
            .HasMaxLength(50)
            .IsRequired();

        modelBuilder.Entity<Inspection>()
            .Property(i => i.DistanceValue)
            .IsRequired();

        modelBuilder.Entity<Inspection>()
            .Property(i => i.FuelLevel)
            .IsRequired();

        modelBuilder.Entity<Inspection>()
            .Property(i => i.FailureReport)
            .IsRequired();

        modelBuilder.Entity<Inspection>()
            .Property(i => i.VehicleFailureObservation)
            .HasMaxLength(500)
            .IsRequired();

        modelBuilder.Entity<Inspection>()
            .Property(i => i.Diagnostic)
            .HasDefaultValue(string.Empty)
            .IsRequired();
        
        modelBuilder.Entity<FileType>()
            .Property(ft => ft.Active)
            .IsRequired()
            .HasDefaultValue(true);

        modelBuilder.Entity<FileType>()
            .Property(ft => ft.Name)
            .IsRequired()
            .HasMaxLength(255);
        
        // FileType
        modelBuilder.Entity<File>()
            .Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(255);

        modelBuilder.Entity<File>()
            .Property(f => f.Format)
            .IsRequired()
            .HasMaxLength(50);

        modelBuilder.Entity<File>()
            .Property(f => f.Path)
            .IsRequired();

        modelBuilder.Entity<File>()
            .Property(f => f.Description)
            .HasDefaultValue(string.Empty);

        modelBuilder.Entity<File>()
            .Property(f => f.Active)
            .IsRequired()
            .HasDefaultValue(true);

        modelBuilder.Entity<File>()
            .Property(f => f.ApplicationUserId)
            .IsRequired();

        modelBuilder.Entity<File>()
            .HasOne(f => f.FileType)
            .WithMany()
            .HasForeignKey(f => f.FileTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<File>()
            .HasOne(f => f.ApplicationUser)
            .WithMany()
            .HasForeignKey(f => f.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Inspection>()
            .HasMany(i => i.Files)
            .WithMany(f => f.Inspections)
            .UsingEntity<Dictionary<string, object>>(
                "inspection_file", // 🔹 Nombre de la tabla intermedia
                j => j.HasOne<File>()
                    .WithMany()
                    .HasForeignKey("FileId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Inspection>()
                    .WithMany()
                    .HasForeignKey("InspectionId")
                    .OnDelete(DeleteBehavior.Cascade)
            );

        modelBuilder.Entity<WorkshopQuote>()
            .HasMany(wq => wq.Files)
            .WithMany(f => f.WorkshopQuotes)
            .UsingEntity<Dictionary<string, object>>(
                "workshop_quote_file",
                j => j.HasOne<File>()
                      .WithMany()
                      .HasForeignKey("FileId")
                      .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<WorkshopQuote>()
                      .WithMany()
                      .HasForeignKey("QuoteId")
                      .OnDelete(DeleteBehavior.Cascade)
            );
        
        modelBuilder.Entity<RequestService>()
            .HasMany(rs => rs.Inspections)
            .WithMany(i => i.RequestServices) // Necesitamos agregar esta prop. de navegación en Inspection (abajo te explico)
            .UsingEntity<Dictionary<string, object>>(
                "request_service_inspection",
                j => j.HasOne<Inspection>()
                    .WithMany()
                    .HasForeignKey("InspectionId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<RequestService>()
                    .WithMany()
                    .HasForeignKey("RequestServiceId")
                    .OnDelete(DeleteBehavior.Cascade)
            );

        modelBuilder.Entity<RequestService>()
            .HasMany(rs => rs.Files)
            .WithMany(f => f.RequestServices) // También agregar prop. en File
            .UsingEntity<Dictionary<string, object>>(
                "request_service_file",
                j => j.HasOne<File>()
                    .WithMany()
                    .HasForeignKey("FileId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<RequestService>()
                    .WithMany()
                    .HasForeignKey("RequestServiceId")
                    .OnDelete(DeleteBehavior.Cascade)
            );
    }

    public override int SaveChanges()
    {
        SetAuditFields();
        return base.SaveChanges();
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetAuditFields();
        return await base.SaveChangesAsync(cancellationToken);
    }
    
    private void SetAuditFields()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => 
                (e.Entity is AuditableEntityBase || e.Entity is ApplicationUser) &&
                (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            if (entry.Entity is AuditableEntityBase entity)
            {
                entity.UpdatedAt = DateTime.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                }
            }
            else if (entry.Entity is ApplicationUser user)
            {
                user.UpdatedAt = DateTime.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    user.CreatedAt = DateTime.UtcNow;
                }
            }
        }
    }
    
    // Función para convertir nombres a snake_case
    private static string ToSnakeCase(string name)
    {
        return string.Concat(name.Select((x, i) =>
            i > 0 && char.IsUpper(x) ? "_" + x : x.ToString())).ToLower();
    }

}