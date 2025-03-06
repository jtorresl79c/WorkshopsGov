using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorkshopsGov.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using WorkshopsGov.Models.Common;

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
    public DbSet<DiagnosticServiceStatus> DiagnosticServiceStatuses { get; set; }
    public DbSet<DiagnosticStatus> DiagnosticStatuses { get; set; }
    public DbSet<VehicleModel> VehicleModels { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<DiagnosticPart> DiagnosticParts { get; set; }
    
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
        
        modelBuilder.Entity<DiagnosticServiceStatus>()
            .Property(dss => dss.Active)
            .HasDefaultValue(true); // Valor por defecto en la BD
        
        modelBuilder.Entity<DiagnosticStatus>()
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
        
        modelBuilder.Entity<DiagnosticPart>()
            .Property(dp => dp.Active)
            .IsRequired()
            .HasDefaultValue(true);

        modelBuilder.Entity<DiagnosticPart>()
            .Property(dp => dp.Name)
            .IsRequired()
            .HasMaxLength(255);
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
    
    public void SeedData()
    {
        Seeders.BrandSeeder.Seed(this);
        Seeders.SectorSeeder.Seed(this);
        Seeders.ExternalWorkshopSeeder.Seed(this);
        Seeders.VehicleStatusSeeder.Seed(this);
        Seeders.VehicleTypeSeeder.Seed(this);
        Seeders.VehicleFailureSeeder.Seed(this);
        Seeders.DiagnosticPartSeeder.Seed(this);
        Seeders.DiagnosticServiceStatusSeeder.Seed(this);
        Seeders.DiagnosticStatusSeeder.Seed(this);
        Seeders.VehicleModelSeeder.Seed(this);
        Seeders.DepartmentSeeder.Seed(this);
        Seeders.ExternalWorkshopBranchSeeder.Seed(this);
        Seeders.ApplicationUserSeeder.Seed(this);
        Seeders.VehicleSeeder.Seed(this);
        // Añade más seeders aquí según sea necesario
    }
    
    // Función para convertir nombres a snake_case
    private static string ToSnakeCase(string name)
    {
        return string.Concat(name.Select((x, i) =>
            i > 0 && char.IsUpper(x) ? "_" + x : x.ToString())).ToLower();
    }
}