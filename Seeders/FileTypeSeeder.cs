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
                    new FileType { Id = 3, Name = "Video", Active = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
                };

                context.FileTypes.AddRange(fileTypes);
                context.SaveChanges();
            }
        }
    }
}