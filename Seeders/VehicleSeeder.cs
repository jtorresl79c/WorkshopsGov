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
                        ModelId = 3,
                        Engine = "V6 3.5L",
                        SectorId = 2,
                        VehicleTypeId = 3,
                        Active = true
                    },
                    new Vehicle
                    {
                        Oficialia = "Oficialia 3",
                        LicensePlate = "LMN-456",
                        VinNumber = "3FA6P0LU5HR123456",
                        Description = "Pickup de trabajo, color blanco.",
                        DepartmentId = 3,
                        VehicleStatusId = 1,
                        Year = 2015,
                        BrandId = 3,
                        ModelId = 5,
                        Engine = "V8 5.0L",
                        SectorId = 3,
                        VehicleTypeId = 2,
                        Active = true
                    },
                    new Vehicle
                    {
                        Oficialia = "Oficialia 4",
                        LicensePlate = "DEF-321",
                        VinNumber = "5NPE24AF8FH123456",
                        Description = "Sedán ejecutivo, color gris.",
                        DepartmentId = 1,
                        VehicleStatusId = 2,
                        Year = 2019,
                        BrandId = 4,
                        ModelId = 7,
                        Engine = "V4 2.0L",
                        SectorId = 1,
                        VehicleTypeId = 1,
                        Active = true
                    },
                    new Vehicle
                    {
                        Oficialia = "Oficialia 5",
                        LicensePlate = "JKL-654",
                        VinNumber = "1C4RJFBG7FC123456",
                        Description = "Camioneta 4x4, color verde.",
                        DepartmentId = 2,
                        VehicleStatusId = 1,
                        Year = 2017,
                        BrandId = 5,
                        ModelId = 9,
                        Engine = "V6 3.2L",
                        SectorId = 2,
                        VehicleTypeId = 4,
                        Active = true
                    },
                    new Vehicle
                    {
                        Oficialia = "Oficialia 6",
                        LicensePlate = "MNO-987",
                        VinNumber = "JN8AS5MT1DW123456",
                        Description = "Crossover familiar, color azul.",
                        DepartmentId = 3,
                        VehicleStatusId = 2,
                        Year = 2016,
                        BrandId = 6,
                        ModelId = 11,
                        Engine = "V4 2.5L",
                        SectorId = 3,
                        VehicleTypeId = 5,
                        Active = true
                    },
                    new Vehicle
                    {
                        Oficialia = "Oficialia 7",
                        LicensePlate = "QRS-741",
                        VinNumber = "1FTFW1ET0EFA12345",
                        Description = "Pickup doble cabina, color plata.",
                        DepartmentId = 1,
                        VehicleStatusId = 1,
                        Year = 2021,
                        BrandId = 7,
                        ModelId = 13,
                        Engine = "V6 3.0L EcoBoost",
                        SectorId = 1,
                        VehicleTypeId = 2,
                        Active = true
                    },
                    new Vehicle
                    {
                        Oficialia = "Oficialia 8",
                        LicensePlate = "TUV-852",
                        VinNumber = "4T1BF1FK4GU123456",
                        Description = "Sedán híbrido, color blanco perla.",
                        DepartmentId = 2,
                        VehicleStatusId = 2,
                        Year = 2022,
                        BrandId = 8,
                        ModelId = 15,
                        Engine = "Híbrido 2.5L",
                        SectorId = 2,
                        VehicleTypeId = 1,
                        Active = true
                    },
                    new Vehicle
                    {
                        Oficialia = "Oficialia 9",
                        LicensePlate = "WXZ-963",
                        VinNumber = "1GCHK29U44E123456",
                        Description = "Camioneta de carga pesada.",
                        DepartmentId = 3,
                        VehicleStatusId = 1,
                        Year = 2014,
                        BrandId = 9,
                        ModelId = 17,
                        Engine = "V8 Diesel",
                        SectorId = 3,
                        VehicleTypeId = 6,
                        Active = true
                    },
                    new Vehicle
                    {
                        Oficialia = "Oficialia 10",
                        LicensePlate = "YZA-159",
                        VinNumber = "3N1AB7AP4HY123456",
                        Description = "Sedán económico, color vino.",
                        DepartmentId = 1,
                        VehicleStatusId = 2,
                        Year = 2023,
                        BrandId = 10,
                        ModelId = 19,
                        Engine = "V4 1.6L",
                        SectorId = 1,
                        VehicleTypeId = 1,
                        Active = true
                    }
                );
                context.SaveChanges();
            }
        }
    }
}