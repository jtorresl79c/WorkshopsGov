using WorkshopsGov.Data;
using WorkshopsGov.Models;

namespace WorkshopsGov.Seeders
{
    public static class SectorSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Sectors.Any())
            {
                context.Sectors.AddRange(
                    new Sector { Name = "Technology", Active = true },
                    new Sector { Name = "Finance", Active = true }
                );
                context.SaveChanges();
            }
        }
    }
}