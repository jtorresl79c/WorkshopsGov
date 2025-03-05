using WorkshopsGov.Data;
using WorkshopsGov.Models;

namespace WorkshopsGov.Seeders
{
    public static class DiagnosticServiceStatusSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.DiagnosticServiceStatuses.Any())
            {
                context.DiagnosticServiceStatuses.AddRange(
                    new DiagnosticServiceStatus { Name = "Not started", Active = true },
                    new DiagnosticServiceStatus { Name = "In progress ", Active = true }
                );
                context.SaveChanges();
            }
        }
    }
}