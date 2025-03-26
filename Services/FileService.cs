using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Microsoft.EntityFrameworkCore;
using WorkshopsGov.Controllers.Global;
using WorkshopsGov.Controllers.PdfGenerators;
using WorkshopsGov.Data;
using WorkshopsGov.Models;
using DbFile = WorkshopsGov.Models.File;


namespace WorkshopsGov.Services
{
    public class FileService
    {
        private readonly ApplicationDbContext _context;

        public FileService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<DbFile> UploadQuoteFileAsync(IFormFile file, int quoteId, int fileTypeId, string description)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Archivo inválido");

            var userId = Utilidades.GetUsername();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId)
                        ?? throw new Exception("Usuario no encontrado");

            var quote = await _context.WorkshopQuote
                .Include(q => q.Files)
                .FirstOrDefaultAsync(q => q.Id == quoteId)
                ?? throw new Exception("Cotización no encontrada");

            var filename = Path.GetFileNameWithoutExtension(file.FileName);
            var extension = Path.GetExtension(file.FileName);

            // 📁 Ruta: Cotizaciones dentro de la inspección correspondiente
            var inspectionId = quote.InspectionId;
            var folderName = "COTIZACIONES";

            var pathFolder = Utilidades.CreateOrGetDirectoryInsideInspectionDirectory(
                Utilidades.GetFullPathInspection(inspectionId),
                folderName
            );

            var fullPath = Path.Combine(pathFolder, filename + extension);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var archivo = new DbFile
            {
                Name = filename,
                Format = extension,
                Size = file.Length / 1024f,
                Description = description,
                FileTypeId = fileTypeId,
                ApplicationUserId = user.Id,
                Active = true,
                Path = fullPath,
                CreatedAt = DateTime.UtcNow
            };

            _context.Files.Add(archivo);
            quote.Files ??= new List<DbFile>();
            quote.Files.Add(archivo);

            await _context.SaveChangesAsync();
            return archivo;
        }



        //public async Task<DbFile> UploadFileAsync(IFormFile file,
        //int inspectionId, int fileTypeId, string description)
        public async Task<DbFile> UploadFileAsync(
         IFormFile file,
         string folderPath,
         int fileTypeId,
         string description,
         string? relativeEntityId = null)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Archivo inválido");

            var userId = Utilidades.GetUsername();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId)
                        ?? throw new Exception("Usuario no encontrado");

            var filename = Path.GetFileNameWithoutExtension(file.FileName);
            var extension = Path.GetExtension(file.FileName);

            var fullPath = Path.Combine(folderPath, filename + extension);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var archivo = new DbFile
            {
                Name = filename,
                Format = extension,
                Size = file.Length / 1024f,
                Description = description,
                FileTypeId = fileTypeId,
                ApplicationUserId = user.Id,
                Active = true,
                Path = fullPath,
                CreatedAt = DateTime.UtcNow
            };

            _context.Files.Add(archivo);
            await _context.SaveChangesAsync();

            return archivo;
        }
        public (byte[] fileBytes, string contentType, string fileName) DownloadFileInline(int inspectionId, int fileTypeId)
        {
            var inspection = _context.Inspections
                .Include(i => i.Files)
                .FirstOrDefault(i => i.Id == inspectionId)
                ?? throw new Exception("Inspección no encontrada.");

            var archivo = inspection.Files
                .Where(f => f.FileTypeId == fileTypeId && f.Active)
                .OrderByDescending(f => f.Id)
                .FirstOrDefault()
                ?? throw new Exception("Archivo no encontrado o inactivo.");

            if (!System.IO.File.Exists(archivo.Path))
                throw new FileNotFoundException("El archivo no existe en el servidor.");

            var fileBytes = System.IO.File.ReadAllBytes(archivo.Path);

            var contentType = archivo.Format.ToLower() switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" or ".png" => "image/png",
                _ => "application/octet-stream"
            };

            var fileName = archivo.Name + archivo.Format;

            return (fileBytes, contentType, fileName);
        }
        public bool DeleteFile(int fileId)
        {
            var file = _context.Files.FirstOrDefault(f => f.Id == fileId && f.Active);

            if (file == null)
                return false;

            file.Active = false;
            _context.Files.Update(file);
            _context.SaveChanges();
            return true;
        }
        public DbFile Generate(int inspectionId, int fileTypeId)
        {
            var inspection = _context.Inspections
                .Include(i => i.Files)
                .Include(i => i.Vehicle).ThenInclude(v => v.Brand)
                .ThenInclude(v => v.VehicleModels)
                .FirstOrDefault(i => i.Id == inspectionId)
                ?? throw new Exception("Inspección no encontrada.");

            var user = _context.Users.FirstOrDefault(u => u.Id == Utilidades.GetUsername())
                ?? throw new Exception("Usuario no encontrado.");

            // 🔹 Ejecutar el generador adecuado según el tipo
            dynamic result = fileTypeId switch
            {
                var id when id == Utilidades.DB_ARCHIVOTIPOS_ENTREGA_RECEPCION
                    => InspectionFile.GenerateFile(inspection, _context),

                //Ejemplo de otro formato
                var id when id == Utilidades.DB_ARCHIVOTIPOS_MEMO_GENERADA
                     => MemoFile.GenerateFile(inspection, _context),

                _ => throw new Exception("Generador no implementado para este tipo de archivo.")
            };

            // 🔹 Descripción amigable según el tipo
            string descripcion = fileTypeId switch
            {
                var id when id == Utilidades.DB_ARCHIVOTIPOS_ENTREGA_RECEPCION
                    => "Formato de Entrega-Recepción",
                //  Ejemplo de otro tipo
                var id when id == Utilidades.DB_ARCHIVOTIPOS_MEMO_GENERADA
                   => "Formato de Memo",

                _ => "Formato generado automáticamente"
            };

            var file = new Models.File
            {
                Name = Path.GetFileNameWithoutExtension(result.Filename),
                Format = result.Formato,
                Size = 0,
                Description = descripcion,
                FileTypeId = fileTypeId,
                ApplicationUserId = user.Id,
                Active = true,
                Path = result.Ruta,
                CreatedAt = DateTime.UtcNow
            };

            _context.Files.Add(file);
            inspection.Files.Add(file);
            _context.SaveChanges();

            return file;
        }

    }
}
