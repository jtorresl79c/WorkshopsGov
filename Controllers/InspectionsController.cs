using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorkshopsGov.Data;
using WorkshopsGov.Models;
using WorkshopsGov.Services;

namespace WorkshopsGov.Controllers
{
    public class InspectionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InspectionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Inspections
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Inspections.Include(i => i.ApplicationUser).Include(i => i.Department).Include(i => i.ExternalWorkshopBranch).Include(i => i.InspectionService).Include(i => i.InspectionStatus).Include(i => i.Vehicle);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Inspections/Details/5

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            return View();
        }

        [HttpGet("api/inspections/{id}")]
        public async Task<IActionResult> GetInspectionDetails(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "ID inválido" });
            }

            var inspection = await _context.Inspections
                .Include(i => i.ApplicationUser)
                .Include(i => i.Department)
                .Include(i => i.ExternalWorkshopBranch)
                .Include(i => i.InspectionService)
                .Include(i => i.InspectionStatus)
                .Include(i => i.Vehicle)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (inspection == null)
            {
                return NotFound(new { message = "Inspección no encontrada" });
            }

            var inspectionDto = new
            {
                Id = inspection.Id,
                MemoNumber = inspection.MemoNumber,
                InspectionDate = inspection.InspectionDate.ToString("yyyy-MM-dd"),
                CheckInTime = inspection.CheckInTime,
                OperatorName = inspection.OperatorName,
                FailureReport = inspection.FailureReport,
                DistanceUnit = inspection.DistanceUnit,
                DistanceValue = inspection.DistanceValue,
                FuelLevel = inspection.FuelLevel,
                // Relaciones
                ApplicationUser = new
                {
                    Id = inspection.ApplicationUser?.Id,
                    UserName = inspection.ApplicationUser?.UserName,
                    Email = inspection.ApplicationUser?.Email
                },
                Department = new
                {
                    Id = inspection.Department?.Id,
                    Name = inspection.Department?.Name
                },
                ExternalWorkshopBranch = new
                {
                    Id = inspection.ExternalWorkshopBranch?.Id,
                    Name = inspection.ExternalWorkshopBranch?.Name
                },
                InspectionService = new
                {
                    Id = inspection.InspectionService?.Id,
                    Name = inspection.InspectionService?.Name
                },
                InspectionStatus = new
                {
                    Id = inspection.InspectionStatus?.Id,
                    Name = inspection.InspectionStatus?.Name
                },
                Vehicle = new
                {
                    Id = inspection.Vehicle?.Id,
                    Description = inspection.Vehicle?.Description,
                    LicensePlate = inspection.Vehicle?.LicensePlate // CORREGIDO
                },
                TowRequired = inspection.TowRequired
            };

            return Ok(inspectionDto);
        }

        [HttpPost("api/inspections/{id}/generate-pdf")]
        public async Task<IActionResult> GeneratePdf(int id)
        {
            var inspection = await _context.Inspections.FindAsync(id);

            if (inspection == null)
            {
                return NotFound(new { message = "Inspección no encontrada" });
            }

            var fileName = InspectionPdfGenerator.GenerateAndSavePdf(id);

            if (fileName == null)
            {
                return StatusCode(500, new { message = "Error al generar el PDF" });
            }

            var filePath = $"/Formats/{fileName}";
            return Ok(new { fileName, filePath });
        }


        //public Task<IActionResult> Details(int? id)
        //{
        //    //if (id == null)
        //    //{
        //    //    return NotFound();
        //    //}

        //    //var inspection = await _context.Inspections
        //    //    .Include(i => i.ApplicationUser)
        //    //    .Include(i => i.Department)
        //    //    .Include(i => i.ExternalWorkshopBranch)
        //    //    .Include(i => i.InspectionService)
        //    //    .Include(i => i.InspectionStatus)
        //    //    .Include(i => i.Vehicle)
        //    //    .FirstOrDefaultAsync(m => m.Id == id);
        //    //if (inspection == null)
        //    //{
        //    //    return NotFound();
        //    //}

        //    return View();
        //}

        // GET: Inspections/Create
        public IActionResult Create()
        {
            ViewBag.CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", ViewBag.CurrentUserId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name");
            ViewData["ExternalWorkshopBranchId"] = new SelectList(_context.ExternalWorkshopBranches, "Id", "Name");
            ViewData["InspectionServiceId"] = new SelectList(_context.InspectionServices, "Id", "Name");
            ViewData["InspectionStatusId"] = new SelectList(_context.InspectionStatuses, "Id", "Name");
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "Oficialia");
            return View();
        }

        // POST: Inspections/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MemoNumber,InspectionDate,CheckInTime,OperatorName,ApplicationUserId,InspectionServiceId,VehicleId,DepartmentId,ExternalWorkshopBranchId,DistanceUnit,DistanceValue,FuelLevel,FailureReport,VehicleFailureObservation,TowRequired,InspectionStatusId,Diagnostic,Active,CreatedAt,UpdatedAt")] Inspection inspection)
        {

            ModelState.Remove("Vehicle");
            ModelState.Remove("Department");
            ModelState.Remove("ApplicationUser");
            ModelState.Remove("InspectionStatus");
            ModelState.Remove("InspectionService");
            ModelState.Remove("ExternalWorkshopBranch");

            inspection.InspectionStatusId = 1;  // ID por defecto
            inspection.InspectionDate = DateTime.SpecifyKind(inspection.InspectionDate, DateTimeKind.Utc);
            inspection.CreatedAt = DateTime.UtcNow;
            inspection.UpdatedAt = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                _context.Add(inspection);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = inspection.Id });
                //return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", inspection.ApplicationUserId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", inspection.DepartmentId);
            ViewData["ExternalWorkshopBranchId"] = new SelectList(_context.ExternalWorkshopBranches, "Id", "Name", inspection.ExternalWorkshopBranchId);
            ViewData["InspectionServiceId"] = new SelectList(_context.InspectionServices, "Id", "Name", inspection.InspectionServiceId);
            ViewData["InspectionStatusId"] = new SelectList(_context.InspectionStatuses, "Id", "Name", inspection.InspectionStatusId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "Description", inspection.VehicleId);
            return View(inspection);
        }

        // GET: Inspections/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspection = await _context.Inspections.FindAsync(id);
            if (inspection == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", inspection.ApplicationUserId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", inspection.DepartmentId);
            ViewData["ExternalWorkshopBranchId"] = new SelectList(_context.ExternalWorkshopBranches, "Id", "Name", inspection.ExternalWorkshopBranchId);
            ViewData["InspectionServiceId"] = new SelectList(_context.InspectionServices, "Id", "Name", inspection.InspectionServiceId);
            ViewData["InspectionStatusId"] = new SelectList(_context.InspectionStatuses, "Id", "Name", inspection.InspectionStatusId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "Description", inspection.VehicleId);
            return View(inspection);
        }

        // POST: Inspections/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MemoNumber,InspectionDate,CheckInTime,OperatorName,ApplicationUserId,InspectionServiceId,VehicleId,DepartmentId,ExternalWorkshopBranchId,DistanceUnit,DistanceValue,FuelLevel,FailureReport,VehicleFailureObservation,TowRequired,InspectionStatusId,Diagnostic,Active,CreatedAt,UpdatedAt")] Inspection inspection)
        {
            if (id != inspection.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inspection);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InspectionExists(inspection.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", inspection.ApplicationUserId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", inspection.DepartmentId);
            ViewData["ExternalWorkshopBranchId"] = new SelectList(_context.ExternalWorkshopBranches, "Id", "Name", inspection.ExternalWorkshopBranchId);
            ViewData["InspectionServiceId"] = new SelectList(_context.InspectionServices, "Id", "Name", inspection.InspectionServiceId);
            ViewData["InspectionStatusId"] = new SelectList(_context.InspectionStatuses, "Id", "Name", inspection.InspectionStatusId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "Description", inspection.VehicleId);
            return View(inspection);
        }

        // GET: Inspections/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspection = await _context.Inspections
                .Include(i => i.ApplicationUser)
                .Include(i => i.Department)
                .Include(i => i.ExternalWorkshopBranch)
                .Include(i => i.InspectionService)
                .Include(i => i.InspectionStatus)
                .Include(i => i.Vehicle)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inspection == null)
            {
                return NotFound();
            }

            return View(inspection);
        }

        // POST: Inspections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inspection = await _context.Inspections.FindAsync(id);
            if (inspection != null)
            {
                _context.Inspections.Remove(inspection);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InspectionExists(int id)
        {
            return _context.Inspections.Any(e => e.Id == id);
        }
    }
}
