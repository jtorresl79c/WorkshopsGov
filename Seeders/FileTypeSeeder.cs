using WorkshopsGov.Data;
using WorkshopsGov.Models;

namespace WorkshopsGov.Seeders
{
    public static class FileTypeSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.FileTypes.Any())
            {
                var fileTypes = new List<FileType>
                {
                    new FileType { Id = 1, Name = "PDF", Active = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                    new FileType { Id = 2, Name = "Image", Active = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                    new FileType { Id = 3, Name = "Video", Active = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                    new FileType { Id = 4, Name = "ENTREGA_RECEPCION_GENERADA", Active = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                    new FileType { Id = 5, Name = "ENTREGA_RECEPCION", Active = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                    new FileType { Id = 6, Name = "MEMO_GENERADA", Active = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                    new FileType { Id = 7, Name = "MEMO_DIGITALIZADO", Active = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                    new FileType { Id = 8, Name = "COTIZACION_DIGITALIZADA", Active = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                    new FileType { Id = 9, Name = "SOLICITUD_DIGITALIZADA", Active = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }

                };

                context.FileTypes.AddRange(fileTypes);
                context.SaveChanges();
            }
        }
    }
}