using WorkshopsGov.Data;
using WorkshopsGov.Models;

namespace WorkshopsGov.Seeders
{
    public static class InspectionPartSeeder
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
            
            if (!context.InspectionParts.Any())
            {
                foreach (var part in parts)
                {
                    context.InspectionParts.Add(new InspectionPart(part) { Name = part, Active = true });
                }
                context.SaveChanges();
            }
        }
    }
}