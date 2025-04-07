using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkshopsGov.Controllers.Global;
using WorkshopsGov.Data;
using WorkshopsGov.Models;
using WorkshopsGov.Services;

namespace WorkshopsGov.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestServicesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly FileService _fileService;
        public RequestServicesApiController(ApplicationDbContext context, FileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        // GET: api/RequestServicesApi/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var solicitud = await _context.RequestServices
                .Include(r => r.Department)
                .Include(r => r.Vehicle)
                .Include(r => r.ApplicationUser)
                .Include(r => r.Files)
                .Where(r => r.Id == id)
                .FirstOrDefaultAsync();

            if (solicitud == null)
            {
                return NotFound();
            }

            // Buscar si hay archivo digitalizado (tipo 7)
            var archivoDigitalizado = solicitud.Files
                .FirstOrDefault(f => f.FileTypeId == Utilidades.DB_ARCHIVOTIPOS_SOLICITUD_DIGITALIZADA && f.Active);

            return Ok(new
            {
                solicitud.Id,
                solicitud.Folio,
                solicitud.Description,
                solicitud.ReceptionDate,
                solicitud.Active,
                solicitud.CreatedAt,
                solicitud.UpdatedAt,
                Department = solicitud.Department.Name,
                Vehicle = new
                {
                    solicitud.Vehicle.Id,
                    solicitud.Vehicle.Oficialia,
                    solicitud.Vehicle.LicensePlate
                },
                User = solicitud.ApplicationUser.UserName,
                FileDigitalizado = archivoDigitalizado != null ? new
                {
                    archivoDigitalizado.Id,
                    archivoDigitalizado.Name,
                    archivoDigitalizado.Path,
                    archivoDigitalizado.FileTypeId,
                    archivoDigitalizado.CreatedAt
                } : null
            });
        }


        [HttpGet]
        [Route("DownloadSolicitudFile/{id}")]
        public IActionResult DownloadSolicitudFile(int id)
        {
            try
            {
                var request = _context.RequestServices
                    .Include(r => r.Files)
                    .FirstOrDefault(r => r.Id == id)
                    ?? throw new Exception("Solicitud de servicio no encontrada.");

                var archivo = request.Files
                    .Where(f => f.FileTypeId == Utilidades.DB_ARCHIVOTIPOS_SOLICITUD_DIGITALIZADA && f.Active)
                    .OrderByDescending(f => f.Id)
                    .FirstOrDefault()
                    ?? throw new Exception("Archivo de solicitud no encontrado o inactivo.");

                var (fileBytes, contentType, fileName) = _fileService.GetFileData(archivo);

                Response.Headers.Append("Content-Disposition", $"inline; filename=\"{fileName}\"");
                return File(fileBytes, contentType);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al mostrar el archivo", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var file = await _context.Files.FindAsync(id);
                if (file == null)
                    return NotFound("Archivo no encontrado.");

                file.Active = false;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Archivo marcado como inactivo correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al marcar el archivo como inactivo.", error = ex.Message });
            }
        }



    }
}
