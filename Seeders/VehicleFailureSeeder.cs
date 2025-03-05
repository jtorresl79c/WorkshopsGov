using WorkshopsGov.Data;
using WorkshopsGov.Models;

namespace WorkshopsGov.Seeders
{
    public static class VehicleFailureSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.VehicleFailures.Any())
            {
                context.VehicleFailures.AddRange(
                    new VehicleFailure { Description = "Vehicle failure 1", Active = true },
                    new VehicleFailure { Description = "Vehicle failure 2", Active = true }
                );
                context.SaveChanges();
            }
        }
    }
}