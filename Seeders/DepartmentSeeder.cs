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
                    new Department { Name = "C1", SectorId = 1, Active = true },
                    new Department { Name = "C2", SectorId = 1, Active = true },
                    new Department { Name = "C3", SectorId = 2, Active = true }
                );
                context.SaveChanges();
            }
        }
    }
}