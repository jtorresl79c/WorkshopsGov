using WorkshopsGov.Data;
using WorkshopsGov.Models;

namespace WorkshopsGov.Seeders
{
    public static class DepartmentSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Departments.Any())
            {
                context.Departments.AddRange(
                    new Department { Name = "IT Support", SectorId = 1, Active = true },
                    new Department { Name = "Development", SectorId = 1, Active = true },
                    new Department { Name = "Accounting", SectorId = 2, Active = true }
                );
                context.SaveChanges();
            }
        }
    }
}