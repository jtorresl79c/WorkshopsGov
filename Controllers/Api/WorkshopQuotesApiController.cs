using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkshopsGov.Controllers.Global;
using WorkshopsGov.Data;
using WorkshopsGov.Services;

namespace WorkshopsGov.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkshopQuotesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly FileService _fileService;

        public WorkshopQuotesApiController(ApplicationDbContext context, FileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        [HttpGet("by-inspection/{inspectionId}")]
        public async Task<IActionResult> GetQuotesByInspection(int inspectionId)
        {
            // Obtener la inspección actual para verificar su estatus
            var inspection = await _context.Inspections
                .Where(i => i.Id == inspectionId)
                .Select(i => new { i.InspectionStatusId })
                .FirstOrDefaultAsync();

            if (inspection == null)
                return NotFound("Inspección no encontrada.");

            var query = _context.WorkshopQuote
                .Where(q => q.InspectionId == inspectionId && q.Active)
                .Include(q => q.WorkshopBranch)
                    .ThenInclude(b => b.ExternalWorkshop)
                .Include(q => q.QuoteStatus)
                .OrderByDescending(q => q.QuoteDate);

            // Materializar la consulta en una lista
            var quotes = await query.ToListAsync();

            // Si la inspección está en estatus Pendiente de Cotización (4), solo mostrar las enviadas a revisión (QuoteStatusId == 2)
            if (User.IsInRole("Administrator"))
            {
                quotes = quotes.Where(q => q.QuoteStatusId != 1).ToList();
            }

            var result = quotes.Select(q => new
            {
                q.Id,
                q.QuoteNumber,
                q.QuoteDate,
                q.TotalCost,
                q.EstimatedCompletionDate,
                q.QuoteDetails,
                QuoteStatus = q.QuoteStatus.Name,
                WorkshopBranch = q.WorkshopBranch.Name,
                WorkshopName = q.WorkshopBranch.ExternalWorkshop.Name,
                HasFile = q.Files.Any(f => f.FileTypeId == Utilidades.DB_ARCHIVOTIPOS_COTIZACION_DIGITALIZADA && f.Active)
            });

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var workshopQuote = await _context.WorkshopQuote.FindAsync(id);
            if (workshopQuote == null)
                return NotFound();

            workshopQuote.Active = false;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cotización eliminada correctamente." });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuoteDetails(int id)
        {
            var quote = await _context.WorkshopQuote
                .Include(q => q.WorkshopBranch)
                    .ThenInclude(b => b.ExternalWorkshop)
                .Include(q => q.QuoteStatus)
                .Where(q => q.Active)
                .Select(q => new
                {
                    q.Id,
                    q.QuoteNumber,
                    q.QuoteDate,
                    q.TotalCost,
                    q.EstimatedCompletionDate,
                    q.QuoteDetails,
                    QuoteStatus = q.QuoteStatus.Name,
                    WorkshopBranch = q.WorkshopBranch.Name,
                    WorkshopName = q.WorkshopBranch.ExternalWorkshop.Name,
                    HasFile = q.Files.Any(f => f.FileTypeId == Utilidades.DB_ARCHIVOTIPOS_COTIZACION_DIGITALIZADA && f.Active)
                })
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quote == null)
                return NotFound();

            return Ok(quote);
        }

        [HttpGet]
        [Route("DownloadQuoteFile/{quoteId}")]
        [HttpGet]
        public IActionResult DownloadQuoteFile(int quoteId)
        {
            try
            {
                var quote = _context.WorkshopQuote
                    .Include(q => q.Files)
                    .FirstOrDefault(q => q.Id == quoteId)
                    ?? throw new Exception("Cotización no encontrada.");

                var archivo = quote.Files
                    .Where(f => f.FileTypeId == Utilidades.DB_ARCHIVOTIPOS_COTIZACION_DIGITALIZADA && f.Active)
                    .OrderByDescending(f => f.Id)
                    .FirstOrDefault()
                    ?? throw new Exception("Archivo no encontrado o inactivo.");

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


        [HttpPost("send-to-review")]
        public async Task<IActionResult> SendQuotesToReview([FromBody] QuoteReviewRequest request)
        {
            var quotes = await _context.WorkshopQuote
                .Where(q => request.QuoteIds.Contains(q.Id))
                .ToListAsync();

            if (!quotes.Any())
                return BadRequest("No se encontraron cotizaciones.");

            // Actualiza cotizaciones
            foreach (var quote in quotes)
            {
                quote.QuoteStatusId = 2;
            }

            // Cambia estado de la inspección (si quieres asegurarte de que exista)
            var inspection = await _context.Inspections.FirstOrDefaultAsync(i => i.Id == request.InspectionId);
            if (inspection != null)
            {
                inspection.InspectionStatusId = Utilidades.PENDIENTE_REVISION_COTIZACION;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Cotizaciones enviadas a revisión." });
        }

        [HttpPost("update-status")]
        public async Task<IActionResult> UpdateQuoteStatus([FromBody] QuoteStatusUpdateDto dto)
        {
            var quotes = await _context.WorkshopQuote // 👈 Aquí estaba el error
                .Where(q => dto.QuoteIds.Contains(q.Id))
                .ToListAsync();

            if (!quotes.Any()) return NotFound("No se encontraron cotizaciones.");

            foreach (var quote in quotes)
            {
                quote.QuoteStatusId = dto.NewStatus; // 👈 Asegúrate de usar la propiedad correcta
            }

            // Actualiza inspección a "En reparación" si al menos una fue aprobada
            if (dto.NewStatus == 4)
            {
                var inspection = await _context.Inspections.FindAsync(dto.InspectionId);
                if (inspection != null)
                {
                    inspection.InspectionStatusId = 5; // En reparación
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }




        public class QuoteStatusUpdateDto
        {
            public List<int> QuoteIds { get; set; } = new();
            public int NewStatus { get; set; }
            public int InspectionId { get; set; }
        }


        public class QuoteReviewRequest
        {
            public List<int> QuoteIds { get; set; } = new();
            public int InspectionId { get; set; }
        }


    }
}
