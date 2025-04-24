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
                    new VehicleFailure { Description = "Sistema eléctrico", Active = true },
                    new VehicleFailure { Description = "Sistema de Frenos", Active = true },
                    new VehicleFailure { Description = "Sistema de Enfriamiento", Active = true },
                    new VehicleFailure { Description = "Sistema de Combustible", Active = true },
                    new VehicleFailure { Description = "Sistema hidráulico", Active = true },
                    new VehicleFailure { Description = "Fuga (Aire, Hidráulico, Agua)", Active = true },
                    new VehicleFailure { Description = "Motor", Active = true },
                    new VehicleFailure { Description = "Transmisión", Active = true },
                    new VehicleFailure { Description = "Diferencial", Active = true },
                    new VehicleFailure { Description = "Embrague (Clutch)", Active = true },
                    new VehicleFailure { Description = "Suspensión/Muelles", Active = true },
                    new VehicleFailure { Description = "Sistema de Escape/Mofle", Active = true },
                    new VehicleFailure { Description = "Soldadura", Active = true },
                    new VehicleFailure { Description = "Sistema de dirección", Active = true },
                    new VehicleFailure { Description = "Otro", Active = true }
                );
                context.SaveChanges();
            }
        }
    }
}