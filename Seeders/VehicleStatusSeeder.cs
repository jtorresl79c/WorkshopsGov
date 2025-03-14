using WorkshopsGov.Data;
using WorkshopsGov.Models;

namespace WorkshopsGov.Seeders
{
    public static class VehicleStatusSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.VehicleStatuses.Any())
            {
                context.VehicleStatuses.AddRange(
                    new VehicleStatus { Name = "Activo", Active = true },
                    new VehicleStatus { Name = "Inactivo", Active = true }
                );
                context.SaveChanges();
            }
        }
    }
}