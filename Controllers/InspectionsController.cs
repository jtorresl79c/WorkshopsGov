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
using ModelFile = WorkshopsGov.Models.File;

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
                .Include(i => i.Files)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (inspection == null)
            {
                return NotFound(new { message = "Inspección no encontrada" });
            }

            var fileDigitalizado = inspection.Files
              .FirstOrDefault(f => f.FileTypeId == Utilidades.DB_ARCHIVOTIPOS_ENTREGA_RECEPCION_DIGITALIZADA && f.Active);

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
                AvailableBranches = branches
            };

            return Ok(inspectionDto);
        }

        [HttpPost]
        public IActionResult DownloadFileOrGenerateFile(int id) // id = IdInspeccion
        {
            var inspection = _context.Inspections
                .Include(i => i.Files)
                .FirstOrDefault(i => i.Id == id);

            if (inspection == null)
            {
                return NotFound("Inspección no encontrada.");
            }

            // 🔹 Obtener el directorio de inspección en wwwroot/Formats/INSPECTIONS/_ID/RECEPCION_ENTREGA
            string _path = Utilidades.CreateOrGetDirectoryInsideInspectionDirectory(Utilidades.GetFullPathInspection(id), "RECEPCION_ENTREGA");

            // 🔹 Buscar archivo vinculado a la inspección
            var archivo = inspection.Files
                .Where(f => f.FileTypeId == Utilidades.DB_ARCHIVOTIPOS_ENTREGA_RECEPCION && f.Active)
                .OrderByDescending(f => f.Id)
                .FirstOrDefault();

            string file;
            byte[] fileBytes;

            // 🔹 Si ya existe el archivo, usarlo o generar uno nuevo
            if (archivo != null)
            {
                file = Path.Combine(_path, archivo.Name + archivo.Format);
                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
                }
            }

            // 🔹 Generar el nuevo archivo si no existe
            archivo = GenerateEntregaRecepcionFile(inspection.Id);
            file = Path.Combine(_path, archivo.Name + archivo.Format);

            // 🔹 Leer el archivo y devolverlo como respuesta
            string filename = archivo.Name + archivo.Format;
            fileBytes = System.IO.File.ReadAllBytes(file);

            string type = archivo.Format switch
            {
                ".jpg" or ".png" or ".jpeg" => "image/png",
                _ => "application/pdf"
            };

            Response.Headers.Append("Content-Disposition", $"inline; filename=\"{filename}\"");
            return File(fileBytes, type);
        }
        public ModelFile GenerateEntregaRecepcionFile(int id)
        {
            string userId = Utilidades.GetUsername();
            var usuario = _context.Users.FirstOrDefault(u => u.Id == userId);

            var inspection = _context.Inspections
                .Include(i => i.Files)
                .Include(i => i.Vehicle)
                .ThenInclude(v => v.Brand)
                .ThenInclude(v => v.VehicleModels) 
                .FirstOrDefault(i => i.Id == id);

            if (inspection == null)
            {
                throw new Exception("Inspección no encontrada.");
            }

            if (usuario == null)
            {
                throw new Exception("Usuario no encontrado.");
            }

            // 🔹 Generar el archivo PDF
            dynamic GeneratedFileResponse = InspectionFile.GenerateFile(inspection, _context);

            // 🔹 Crear el archivo en la base de datos
            var archivo = new WorkshopsGov.Models.File
            {
                Name = Path.GetFileNameWithoutExtension(GeneratedFileResponse.Filename),
                Format = GeneratedFileResponse.Formato,
                Size = 0, // Opcional: Puedes calcular el tamaño real con FileInfo
                Description = "Formato de Entrega-Recepción",
                FileTypeId = Utilidades.DB_ARCHIVOTIPOS_ENTREGA_RECEPCION,
                ApplicationUserId = usuario.Id,
                Active = true,
                Path = GeneratedFileResponse.Ruta,
                CreatedAt = DateTime.UtcNow
            };

            _context.Files.Add(archivo);
            _context.SaveChanges();

            // 🔹 Vincular el archivo con la inspección en la tabla intermedia `inspection_file`
            inspection.Files.Add(archivo);
            _context.SaveChanges();

            return archivo;
        }

        [HttpPost]
        public IActionResult UploadFile(IFormFile file, int id)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Debe seleccionar un archivo para subir.");
            }

            // 🔹 Obtener el usuario actual
            string userId = Utilidades.GetUsername();
            var usuario = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (usuario == null)
            {
                return Unauthorized("Usuario no encontrado.");
            }

            // 🔹 Buscar la inspección
            var inspection = _context.Inspections.Include(i => i.Files).FirstOrDefault(i => i.Id == id);
            if (inspection == null)
            {
                return NotFound("Inspección no encontrada.");
            }

            // 🔹 Generar el path donde se guardará el archivo
            string filename = Path.GetFileNameWithoutExtension(file.FileName);
            string fileExtension = Path.GetExtension(file.FileName).ToLower();
            string directoryPath = Utilidades.CreateOrGetDirectoryInsideInspectionDirectory(Utilidades.GetFullPathInspection(id), "RECEPCION_ENTREGA");
            string fullPath = Path.Combine(directoryPath, filename + fileExtension);

            // 🔹 Guardar el archivo en el servidor
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            // 🔹 Crear el registro en la base de datos
            var archivo = new WorkshopsGov.Models.File
            {
                Name = filename,
                Format = fileExtension,
                Size = file.Length / 1024f, // Convertir a KB
                Description = "Entrega-Recepción Digitalizada",
                FileTypeId = Utilidades.DB_ARCHIVOTIPOS_ENTREGA_RECEPCION_DIGITALIZADA, // 🔹 Tipo específico
                ApplicationUserId = usuario.Id,
                Active = true,
                Path = fullPath,
                CreatedAt = DateTime.UtcNow
            };

            _context.Files.Add(archivo);
            _context.SaveChanges(); // 🔹 Guardar primero el archivo en `Files`

            // 🔹 Ahora, agregarlo a la inspección para que EF lo relacione en `inspection_file`
            inspection.Files.Add(archivo);
            _context.SaveChanges(); // 🔹 Esto genera automáticamente la relación en `inspection_file`

            return Ok(new { message = "Archivo subido exitosamente.", filePath = fullPath });
        }

        [HttpGet]
        public IActionResult DownloadFile(int id, int fileTypeId)
        {
            // 🔹 Buscar la inspección y asegurarse de que existen archivos asociados
            var inspection = _context.Inspections
                .Include(i => i.Files)
                .FirstOrDefault(i => i.Id == id);

            if (inspection == null)
            {
                return NotFound("Inspección no encontrada.");
            }

            // 🔹 Buscar el archivo más reciente con el tipo de archivo especificado
            var archivo = inspection.Files
                .Where(f => f.FileTypeId == fileTypeId)
                .OrderByDescending(f => f.Id)
                .FirstOrDefault();

            if (archivo == null)
            {
                return NotFound("No se encontró un archivo de este tipo para esta inspección.");
            }

            // 🔹 Obtener la ruta real del archivo
            string filePath = archivo.Path;

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("El archivo no existe en el servidor.");
            }

            // 🔹 Leer el archivo desde el disco
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            string contentType = "application/octet-stream"; // Tipo de archivo por defecto

            // 🔹 Determinar el tipo MIME basado en la extensión del archivo
            switch (archivo.Format.ToLower())
            {
                case ".pdf":
                    contentType = "application/pdf";
                    break;
                case ".jpg":
                case ".jpeg":
                case ".png":
                    contentType = "image/png";
                    break;
                default:
                    contentType = "application/octet-stream"; // Otros archivos
                    break;
            }

            // 🔹 Configurar la cabecera para mostrar el archivo en el navegador
            Response.Headers.Append("Content-Disposition", $"inline; filename=\"{archivo.Name}{archivo.Format}\"");

            return File(fileBytes, contentType);
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

        // POST: Inspections/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        [HttpDelete("Inspections/DeleteFile/{fileId}/{fileTypeId}")]
        public IActionResult DeleteFile(int fileId, int fileTypeId)
        {
            var file = _context.Files
                .FirstOrDefault(f => f.Id == fileId && f.FileTypeId == fileTypeId && f.Active);

            if (file == null)
            {
                return NotFound("Archivo no encontrado o ya está inactivo.");
            }

            try
            {
                file.Active = false;
                _context.Entry(file).State = EntityState.Modified;
                _context.SaveChanges();

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
