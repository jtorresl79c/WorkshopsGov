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
                        Name = "1500",
                        BrandId = 1,
                        Active = true
                    },
                    new VehicleModel
                    {
                        Name = "2500",
                        BrandId = 1,
                        Active = true
                    },
                    new VehicleModel
                    {
                        Name = "F150",
                        BrandId = 2,
                        Active = true
                    },
                    new VehicleModel
                    {
                        Name = "pickup Heavy Duty",
                        BrandId = 2,
                        Active = true
                    }
                );
                context.SaveChanges();
            }
        }
    }
}