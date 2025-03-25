using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkshopsGov.Controllers.Global;
using WorkshopsGov.Data;

namespace WorkshopsGov.Controllers.Api
{
    public class WorkshopInspectionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkshopInspectionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "External_Workshop")]
        [HttpGet("api/reviewcenter/my-inspections")]
        public async Task<IActionResult> GetInspectionsForCurrentWorkshop()
        {
            var userId = Utilidades.GetUsername(); // Tu helper ya lo hace
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            // 🔍 Buscar los talleres asignados al usuario
            var workshops = await _context.ExternalWorkshops
                .Include(w => w.ExternalWorkshopBranches)
                .Where(w => w.Users.Any(u => u.Id == userId))
                .ToListAsync();

            // 🔎 Obtener IDs de sucursales (branches) del taller
            var branchIds = workshops
                .SelectMany(w => w.ExternalWorkshopBranches)
                .Select(b => b.Id)
                .ToList();

            // 🛠️ Buscar inspecciones asignadas a esas sucursales
            var inspecciones = await _context.Inspections
                .Include(i => i.Vehicle)
                .Include(i => i.ExternalWorkshopBranch)
                .Include(i => i.InspectionStatus)
                .Where(i => i.InspectionStatusId == 2 && branchIds.Contains(i.ExternalWorkshopBranchId))
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

    }
}
