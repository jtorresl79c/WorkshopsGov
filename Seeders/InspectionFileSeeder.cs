using Microsoft.EntityFrameworkCore;
using WorkshopsGov.Data;

namespace WorkshopsGov.Seeders
{
    public static class InspectionFileSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            var inspection = context.Inspections.Include(inspection => inspection.Files).FirstOrDefault();
            var file = context.Files.FirstOrDefault();
            
            var inspectonFiles = inspection.Files;

            if (inspectonFiles.Any()) return;
            
            if (inspection != null && file != null)
            {
                inspection.Files.Add(file);
                context.SaveChanges();
            }
        }
    }
}