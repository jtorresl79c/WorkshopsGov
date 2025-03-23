using WorkshopsGov.Data;
using WorkshopsGov.Models;

namespace WorkshopsGov.Seeders
{
    public static class ExternalWorkshopSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.ExternalWorkshops.Any())
            {
                context.ExternalWorkshops.AddRange(
                    new ExternalWorkshop { Name = "Taller Automotriz Alex", Active = true },
                    new ExternalWorkshop { Name = "Taller automotriz Cars Ebrios Boys", Active = true }
                );
                context.SaveChanges();
            }
        }
    }
}