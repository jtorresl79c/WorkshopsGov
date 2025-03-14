using WorkshopsGov.Data;
using WorkshopsGov.Models;

namespace WorkshopsGov.Seeders
{
    public static class InspectionServiceSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.InspectionServices.Any())
            {
                context.InspectionServices.AddRange(
                    new InspectionService { Name = "Service 1", Active = true },
                    new InspectionService { Name = "Service 2 ", Active = true }
                );
                context.SaveChanges();
            }
        }
    }
}