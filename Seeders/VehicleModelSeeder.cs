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
                    new VehicleModel { Name = "F-150", BrandId = 1, Active = true },              // Ford
                    new VehicleModel { Name = "Ranger", BrandId = 1, Active = true },
                    
                    new VehicleModel { Name = "1500", BrandId = 2, Active = true },               // Ram
                    new VehicleModel { Name = "2500", BrandId = 2, Active = true },

                    new VehicleModel { Name = "Silverado", BrandId = 3, Active = true },          // Chevrolet
                    new VehicleModel { Name = "Colorado", BrandId = 3, Active = true },

                    new VehicleModel { Name = "Hilux", BrandId = 4, Active = true },              // Toyota
                    new VehicleModel { Name = "Tacoma", BrandId = 4, Active = true },

                    new VehicleModel { Name = "Frontier", BrandId = 5, Active = true },           // Nissan
                    new VehicleModel { Name = "Titan", BrandId = 5, Active = true },

                    new VehicleModel { Name = "Ridgeline", BrandId = 6, Active = true },          // Honda
                    new VehicleModel { Name = "CR-V", BrandId = 6, Active = true },

                    new VehicleModel { Name = "Santa Fe", BrandId = 7, Active = true },           // Hyundai
                    new VehicleModel { Name = "Tucson", BrandId = 7, Active = true },

                    new VehicleModel { Name = "Amarok", BrandId = 8, Active = true },             // Volkswagen
                    new VehicleModel { Name = "Saveiro", BrandId = 8, Active = true },

                    new VehicleModel { Name = "Sportage", BrandId = 9, Active = true },           // Kia
                    new VehicleModel { Name = "Sorento", BrandId = 9, Active = true },

                    new VehicleModel { Name = "BT-50", BrandId = 10, Active = true },             // Mazda
                    new VehicleModel { Name = "CX-5", BrandId = 10, Active = true }
                );
                context.SaveChanges();
            }
        }
    }
}