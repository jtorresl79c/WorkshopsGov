using WorkshopsGov.Data;
using WorkshopsGov.Models;

namespace WorkshopsGov.Seeders
{
    public static class DiagnosticStatusSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.DiagnosticStatuses.Any())
            {
                context.DiagnosticStatuses.AddRange(
                    new DiagnosticStatus { Name = "Motor destroyed DS", Active = true },
                    new DiagnosticStatus { Name = "Need new Tires DS ", Active = true }
                );
                context.SaveChanges();
            }
        }
    }
}