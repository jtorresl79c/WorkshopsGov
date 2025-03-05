using WorkshopsGov.Data;
using WorkshopsGov.Models;

namespace WorkshopsGov.Seeders
{
    public static class ExternalWorkshopBranchSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.ExternalWorkshopBranches.Any())
            {
                context.ExternalWorkshopBranches.AddRange(
                    new ExternalWorkshopBranch { Name = "Branch 1", ExternalWorkshopId = 1},
                    new ExternalWorkshopBranch { Name = "Branch 2", ExternalWorkshopId = 1},
                    new ExternalWorkshopBranch { Name = "Branch 3", ExternalWorkshopId = 2},
                    new ExternalWorkshopBranch { Name = "Branch 4", ExternalWorkshopId = 2}
                );
                context.SaveChanges();
            }
        }
    }
}