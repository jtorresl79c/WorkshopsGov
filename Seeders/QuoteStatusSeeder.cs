using WorkshopsGov.Data;
using WorkshopsGov.Models;

namespace WorkshopsGov.Seeders
{
    public static class QuoteStatusSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.WorkshopQuoteStatus.Any())
            {
                context.WorkshopQuoteStatus.AddRange(
                    new WorkshopQuoteStatus { Name = "Capturada", Active = true },
                    new WorkshopQuoteStatus { Name = "En revision", Active = true },
                    new WorkshopQuoteStatus { Name = "Rechazado ", Active = true }
                );
                context.SaveChanges();
            }
        }
    }
}