using WorkshopsGov.Data;
using WorkshopsGov.Models;

namespace WorkshopsGov.Seeders
{
    public static class VehicleTypeSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.VehicleTypes.Any())
            {
                context.VehicleTypes.AddRange(
                    new VehicleType { Name = "Sedan VT", Active = true },
                    new VehicleType { Name = "Pickup VT", Active = true }
                );
                context.SaveChanges();
            }
        }
    }
}