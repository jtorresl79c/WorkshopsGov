using WorkshopsGov.Data;
using WorkshopsGov.Models;

namespace WorkshopsGov.Seeders
{
    public static class InspectionStatusSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.InspectionStatuses.Any())
            {
                context.InspectionStatuses.AddRange(
                    new InspectionStatus { Id = 1, Name = "Capturada", Active = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                    new InspectionStatus { Id = 2, Name = "Taller Asignado", Active = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                    new InspectionStatus { Id = 3, Name = "ELIMINADO ", Active = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
                );
                context.SaveChanges();
            }
        }
    }
}