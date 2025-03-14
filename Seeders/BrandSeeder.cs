using WorkshopsGov.Data;
using WorkshopsGov.Models;

namespace WorkshopsGov.Seeders
{
    public static class BrandSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Brands.Any())
            {
                context.Brands.AddRange(
                    new Brand { Name = "Ford", Active = true },
                    new Brand { Name = "Ram", Active = true }
                );
                context.SaveChanges();
            }
        }
    }
}