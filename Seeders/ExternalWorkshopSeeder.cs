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
                    new ExternalWorkshop { Name = "Orion EW", Active = true },
                    new ExternalWorkshop { Name = "Cygnus EW", Active = true }
                );
                context.SaveChanges();
            }
        }
    }
}