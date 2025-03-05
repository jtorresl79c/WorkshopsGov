using WorkshopsGov.Data;
using WorkshopsGov.Models;

namespace WorkshopsGov.Seeders
{
    public static class VehicleModelSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.VehicleModels.Any())
            {
                context.VehicleModels.AddRange(
                    new VehicleModel
                    {
                        Name = "Civic",
                        BrandId = 1,
                        Active = true
                    },
                    new VehicleModel
                    {
                        Name = "Accord",
                        BrandId = 1,
                        Active = true
                    },
                    new VehicleModel
                    {
                        Name = "Corolla",
                        BrandId = 2,
                        Active = true
                    },
                    new VehicleModel
                    {
                        Name = "Hilux",
                        BrandId = 2,
                        Active = true
                    }
                );
                context.SaveChanges();
            }
        }
    }
}