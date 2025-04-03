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
                    new Brand { Name = "Ram", Active = true },
                    new Brand { Name = "Chevrolet", Active = true },
                    new Brand { Name = "Toyota", Active = true },
                    new Brand { Name = "Nissan", Active = true },
                    new Brand { Name = "Honda", Active = true },
                    new Brand { Name = "Hyundai", Active = true },
                    new Brand { Name = "Volkswagen", Active = true },
                    new Brand { Name = "Kia", Active = true },
                    new Brand { Name = "Mazda", Active = true }
                );
                context.SaveChanges();
            }
        }
    }
}