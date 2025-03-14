using WorkshopsGov.Data;
using WorkshopsGov.Models;

namespace WorkshopsGov.Seeders
{
    public static class InspectionServiceStatusSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.InspectionServiceStatuses.Any())
            {
                context.InspectionServiceStatuses.AddRange(
                    new InspectionServiceStatus { Name = "Not started", Active = true },
                    new InspectionServiceStatus { Name = "In progress ", Active = true }
                );
                context.SaveChanges();
            }
        }
    }
}