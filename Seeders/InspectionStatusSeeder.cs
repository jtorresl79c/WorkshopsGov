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
                    new InspectionStatus { Name = "Capturada", Active = true },
                    new InspectionStatus { Name = "Taller Asignado", Active = true },
                    new InspectionStatus { Name = "ELIMINADO ", Active = true }
                );
                context.SaveChanges();
            }
        }
    }
}