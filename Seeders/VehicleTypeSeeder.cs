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
                    new VehicleType { Name = "Sedán", Active = true },
                    new VehicleType { Name = "Pickup", Active = true },
                    new VehicleType { Name = "SUV", Active = true },
                    new VehicleType { Name = "Camioneta", Active = true },
                    new VehicleType { Name = "Crossover", Active = true },
                    new VehicleType { Name = "Hatchback", Active = true },
                    new VehicleType { Name = "Convertible", Active = true },
                    new VehicleType { Name = "Van", Active = true },
                    new VehicleType { Name = "Minivan", Active = true },
                    new VehicleType { Name = "Coupé", Active = true }
                );
                context.SaveChanges();
            }
        }
    }
}