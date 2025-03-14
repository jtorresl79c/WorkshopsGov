using WorkshopsGov.Data;
using WorkshopsGov.Models;

namespace WorkshopsGov.Seeders
{
    public static class InspectionStatusSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.InspectionStatuses.Any())
            {
                context.InspectionStatuses.AddRange(
                    new InspectionStatus { Name = "Motor destroyed DS", Active = true },
                    new InspectionStatus { Name = "Need new Tires DS ", Active = true }
                );
                context.SaveChanges();
            }
        }
    }
}