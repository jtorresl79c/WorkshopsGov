using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkshopsGov.Data;
using WorkshopsGov.Models;

namespace WorkshopsGov.Controllers
{
    public class ReviewCenterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewCenterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Esta vista es la que se conectará con Vue
        [HttpGet]
        public IActionResult Index()
        {
            return View(); // Renderiza la vista Razor vacía donde Vue montará
        }

        // Endpoint de prueba para Vue (solo para testear conexión)
        [HttpGet("api/reviewcenter/test")]
        public IActionResult Test()
        {
            return Ok(new { message = "Conexión con ReviewCenter funcionando correctamente." });
        }

        // Aquí vas a ir agregando tus endpoints reales
        // GET: api/reviewcenter/assigned
        [HttpGet("api/reviewcenter/assigned")]
        public async Task<IActionResult> GetAssignedInspections()
        {
            var inspecciones = await _context.Inspections
                .Include(i => i.Vehicle)
                .Include(i => i.ExternalWorkshopBranch)
                .Include(i => i.InspectionStatus)
                .Where(i => i.InspectionStatusId == 2) // ✅ ID 2 = Asignada
                .Select(i => new
                {
                    i.Id,
                    i.MemoNumber,
                    i.InspectionDate,
                    Taller = i.ExternalWorkshopBranch != null ? i.ExternalWorkshopBranch.Name : "Sin taller",
                    Vehiculo = i.Vehicle.Oficialia,
                    Estado = i.InspectionStatus.Name,
                    EstatusId = i.InspectionStatusId
                })
                .ToListAsync();

            return Ok(inspecciones);
        }

        // A futuro: /pending-quotes, /in-repair, /verified...
    }
}
