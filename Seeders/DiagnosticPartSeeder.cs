using WorkshopsGov.Data;
using WorkshopsGov.Models;

namespace WorkshopsGov.Seeders
{
    public static class DiagnosticPartSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            List<string> parts =
            [
                "Engine",
                "Transmission",
                "Brakes",
                "Suspension",
                "Wheels",
                "Steering Wheel",
                "Exhaust System",
                "Fuel Tank",
                "Radiator",
                "Battery"
            ];
            
            if (!context.DiagnosticParts.Any())
            {
                foreach (var part in parts)
                {
                    context.DiagnosticParts.Add(new DiagnosticPart(part) { Name = part, Active = true });
                }
                context.SaveChanges();
            }
        }
    }
}