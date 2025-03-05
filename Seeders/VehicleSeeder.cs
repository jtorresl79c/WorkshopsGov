using WorkshopsGov.Data;
using WorkshopsGov.Models;

namespace WorkshopsGov.Seeders
{
    public static class VehicleSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Vehicles.Any())
            {
                context.Vehicles.AddRange(
                    new Vehicle
                    {
                        Oficialia = "Oficialia 1",
                        LicensePlate = "ABC-123",
                        VinNumber = "1HGCM82633A123456",
                        Description = "Sedán compacto, color rojo.",
                        DepartmentId = 1,
                        VehicleStatusId = 1,
                        Year = 2020,
                        BrandId = 1,
                        ModelId = 1,
                        Engine = "V4 1.5L Turbo",
                        SectorId = 1,
                        VehicleTypeId = 1,
                        Active = true
                    },
                    new Vehicle
                    {
                        Oficialia = "Oficialia 2",
                        LicensePlate = "XYZ-789",
                        VinNumber = "2T1BURHE9KC456789",
                        Description = "SUV de lujo, color negro.",
                        DepartmentId = 2,
                        VehicleStatusId = 2,
                        Year = 2018,
                        BrandId = 2,
                        ModelId = 2,
                        Engine = "V6 3.5L",
                        SectorId = 2,
                        VehicleTypeId = 2,
                        Active = true
                    },
                    new Vehicle
                    {
                        Oficialia = "Oficialia 3",
                        LicensePlate = "LMN-456",
                        VinNumber = "3FA6P0LU5HR123456",
                        Description = "Pickup de trabajo, color blanco.",
                        DepartmentId = 2,
                        VehicleStatusId = 2,
                        Year = 2015,
                        BrandId = 2,
                        ModelId = 2,
                        Engine = "V8 5.0L",
                        SectorId = 2,
                        VehicleTypeId = 2,
                        Active = false
                    }
                );
                context.SaveChanges();
            }
        }
    }
}