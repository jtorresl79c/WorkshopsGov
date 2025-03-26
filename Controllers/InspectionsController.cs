using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorkshopsGov.Controllers.Global;
using WorkshopsGov.Controllers.PdfGenerators;
using WorkshopsGov.Data;
using WorkshopsGov.Models;
using WorkshopsGov.Services;
using ModelFile = WorkshopsGov.Models.File;

namespace WorkshopsGov.Controllers
{
    public class InspectionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly FileService _fileService;

        public InspectionsController(ApplicationDbContext context, FileService fileService)
        {
            _context = context;
            _fileService = fileService;
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
                .Include(i => i.Files)
                .FirstOrDefaultAsync(m => m.Id == id);

            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;


            if (inspection == null)
            {
                return NotFound(new { message = "Inspección no encontrada" });
            }

            var fileDigitalizado = inspection.Files
              .FirstOrDefault(f => f.FileTypeId == Utilidades.DB_ARCHIVOTIPOS_ENTREGA_RECEPCION_DIGITALIZADA && f.Active);

            var fileMemoDigitalizado = inspection.Files
                .FirstOrDefault(f => f.FileTypeId == Utilidades.DB_ARCHIVOTIPOS_MEMO_DIGITALIZADO && f.Active);

            var branches = await _context.ExternalWorkshopBranches
            .Include(b => b.ExternalWorkshop)
            .Where(b => b.ExternalWorkshop.Id != 1) // ⛔ Excluir taller con ID 1
            .Select(b => new
            {
                Id = b.Id,
                Name = b.Name,
                WorkshopName = b.ExternalWorkshop.Name
            })
            .ToListAsync();


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
                TowRequired = inspection.TowRequired,
                FileDigitalizado = fileDigitalizado != null ? new
                {
                    Id = fileDigitalizado.Id,
                    Name = fileDigitalizado.Name,
                    Path = fileDigitalizado.Path,
                    UploadedAt = fileDigitalizado.CreatedAt,
                    FileTypeId = fileDigitalizado.FileTypeId,
                } : null,
                FileMemoDigitalizado = fileMemoDigitalizado != null ? new
                {
                    Id = fileMemoDigitalizado.Id,
                    Name = fileMemoDigitalizado.Name,
                    Path = fileMemoDigitalizado.Path,
                    UploadedAt = fileMemoDigitalizado.CreatedAt,
                    FileTypeId = fileMemoDigitalizado.FileTypeId,
                } : null,
                AvailableBranches = branches,
                CurrentUserRole = userRole
            };

            return Ok(inspectionDto);
        }

        [HttpPost]
        public IActionResult DownloadFileOrGenerateFile(int id, int fileTypeId)
        {
            var inspection = _context.Inspections
                .Include(i => i.Files)
                .FirstOrDefault(i => i.Id == id);

            if (inspection == null)
                return NotFound("Inspección no encontrada.");

            //string path = Utilidades.CreateOrGetDirectoryInsideInspectionDirectory(
            //    Utilidades.GetFullPathInspection(id), "RECEPCION_ENTREGA");
            string folderName = Utilidades.GetFolderNameByFileTypeId(fileTypeId);

            string path = Utilidades.CreateOrGetDirectoryInsideInspectionDirectory(
                Utilidades.GetFullPathInspection(id), folderName);

            var archivo = inspection.Files
                .Where(f => f.FileTypeId == fileTypeId && f.Active)
                .OrderByDescending(f => f.Id)
                .FirstOrDefault();

            string file;
            if (archivo != null)
            {
                file = Path.Combine(path, archivo.Name + archivo.Format);
                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
                }
            }

            // Puedes usar tu servicio aquí también (con switch por tipo si aplica)
            archivo = _fileService.Generate(id, fileTypeId);

            file = Path.Combine(path, archivo.Name + archivo.Format);
            var fileBytes = System.IO.File.ReadAllBytes(file);

            string type = archivo.Format switch
            {
                ".jpg" or ".png" or ".jpeg" => "image/png",
                _ => "application/pdf"
            };

            Response.Headers.Append("Content-Disposition", $"inline; filename=\"{archivo.Name}{archivo.Format}\"");
            return File(fileBytes, type);
        }


        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file, int id, int fileTypeId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Debe seleccionar un archivo para subir.");

            try
            {
                var description = Utilidades.GetFileTypeDescription(fileTypeId);

                var inspection = await _context.Inspections
                .Include(i => i.Files)
                .FirstOrDefaultAsync(i => i.Id == id)
                ?? throw new Exception("Inspección no encontrada");

                var folderName = Utilidades.GetFolderNameByFileTypeId(fileTypeId);
                var pathFolder = Utilidades.CreateOrGetDirectoryInsideInspectionDirectory(
                    Utilidades.GetFullPathInspection(id), folderName
                );


                var archivo = await _fileService.UploadFileAsync(
                    file, 
                    pathFolder, 
                    fileTypeId, 
                    description
                    );

                // genera la relación entre el archivo subido y la inspección
                inspection.Files.Add(archivo);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Archivo subido exitosamente.", filePath = archivo.Path });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al subir archivo", error = ex.Message });
            }
        }


        [HttpGet]
        public IActionResult DownloadFile(int id, int fileTypeId)
        {
            try
            {
                var (fileBytes, contentType, fileName) = _fileService.DownloadFileInline(id, fileTypeId);
                // 🔹 Mostrar directamente en el navegador (no descargar)
                Response.Headers.Append("Content-Disposition", $"inline; filename=\"{fileName}\"");
                return File(fileBytes, contentType);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al mostrar el archivo.", error = ex.Message });
            }
        }

        [HttpPost("api/inspections/{id}/assign-workshop")]
        public async Task<IActionResult> AssignWorkshop(int id, [FromBody] AssignWorkshopDto dto)
        {
            if (dto.BranchId <= 0)
            {
                return BadRequest(new { message = "Sucursal inválida." });
            }

            var inspection = await _context.Inspections.FindAsync(id);

            if (inspection == null)
            {
                return NotFound(new { message = "Inspección no encontrada." });
            }

            // Asignar la sucursal y actualizar estatus
            inspection.ExternalWorkshopBranchId = dto.BranchId;
            inspection.InspectionStatusId = 2; // Estado: Por aprobar cotización
            inspection.UpdatedAt = DateTime.UtcNow;

            try
            {
                _context.Update(inspection);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Sucursal asignada correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al asignar la sucursal.", error = ex.Message });
            }
        }
        public class AssignWorkshopDto
        {
            public int BranchId { get; set; }
        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemoNumber,InspectionDate,CheckInTime,OperatorName,ApplicationUserId,InspectionServiceId,VehicleId,DepartmentId,DistanceUnit,DistanceValue,FuelLevel,FailureReport,VehicleFailureObservation,TowRequired,InspectionStatusId,Diagnostic,Active,CreatedAt,UpdatedAt")] Inspection inspection)

        //public async Task<IActionResult> Create([Bind("Id,MemoNumber,InspectionDate,CheckInTime,OperatorName,ApplicationUserId,InspectionServiceId,VehicleId,DepartmentId,DistanceUnit,DistanceValue,FuelLevel,FailureReport,VehicleFailureObservation,TowRequired,InspectionStatusId,Diagnostic,Active,CreatedAt,UpdatedAt")] Inspection inspection)
        //public async Task<IActionResult> Create([Bind("Id,MemoNumber,InspectionDate,CheckInTime,OperatorName,ApplicationUserId,InspectionServiceId,VehicleId,DepartmentId,ExternalWorkshopBranchId,DistanceUnit,DistanceValue,FuelLevel,FailureReport,VehicleFailureObservation,TowRequired,InspectionStatusId,Diagnostic,Active,CreatedAt,UpdatedAt")] Inspection inspection)
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
            inspection.ExternalWorkshopBranchId = 1; 

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
        [HttpDelete("Inspections/DeleteFile/{fileId}/{fileTypeId}")]
        public IActionResult DeleteFile(int fileId, int fileTypeId)
        {
            try
            {
                bool deleted = _fileService.DeleteFile(fileId);

                if (!deleted)
                    return NotFound("Archivo no encontrado o ya está inactivo.");

                return Ok(new { message = "Archivo marcado como inactivo correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al marcar el archivo como inactivo.", error = ex.Message });
            }
        }
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
