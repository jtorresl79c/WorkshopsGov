using WorkshopsGov.Data;
using FileEntity = WorkshopsGov.Models.File; // 🔹 Alias para evitar conflicto

namespace WorkshopsGov.Seeders
{
    public static class FileSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            var foundUser = context.Users.FirstOrDefault();
            if (foundUser == null) return;

            if (!context.Files.Any())
            {
                var files = new List<FileEntity> // 🔹 Ahora usamos FileEntity
                {
                    new FileEntity
                    {
                        Id = 1,
                        Name = "Document_1",
                        Format = "pdf",
                        Size = 2.5f,
                        Description = "Manual de usuario",
                        Path = "/storage/documents/manual.pdf",
                        FileTypeId = 1,
                        ApplicationUserId = foundUser.Id,
                        Active = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new FileEntity
                    {
                        Id = 2,
                        Name = "Screenshot",
                        Format = "png",
                        Size = 1.2f,
                        Description = "Captura de pantalla del error",
                        Path = "/storage/images/screenshot.png",
                        FileTypeId = 2,
                        ApplicationUserId = foundUser.Id,
                        Active = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };

                context.Files.AddRange(files);
                context.SaveChanges();
            }
        }
    }
}