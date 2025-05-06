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
                    new Sector { Name = "SECTOR LIBERTAD (Transferencia)", Active = true },
                    new Sector { Name = "SECTOR PRESA (Transferencia)", Active = true },
                    new Sector { Name = "SECTOR SAB (Valle Sur Transferencia)", Active = true },
                    new Sector { Name = "SECTOR OTAY", Active = true },
                    new Sector { Name = "SECTOR PLAYAS", Active = true },
                    new Sector { Name = "SECTOR MAQUINARIA Y EQUIPO", Active = true },
                    new Sector { Name = "SECTOR LA MESA", Active = true },
                    new Sector { Name = "PALACIO MUNICIPAL", Active = true },
                    new Sector { Name = "SEGURIDAD PUBLICA", Active = true },
                    new Sector { Name = "TALLER CENTRAL (Vía rápida)", Active = true }
                );
                context.SaveChanges();
            }
        }
    }
}