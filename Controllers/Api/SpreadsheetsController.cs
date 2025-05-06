using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkshopsGov.Data;

namespace WorkshopsGov.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpreadsheetsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SpreadsheetsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("inspections")]
        public async Task<IActionResult> ExportInspectionsToExcel([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int? serviceId)
        {
            var query = _context.Inspections
                .Include(i => i.ApplicationUser)
                .Include(i => i.Department)
                .Include(i => i.ExternalWorkshopBranch)
                .Include(i => i.InspectionService)
                .Include(i => i.InspectionStatus)
                .Include(i => i.Vehicle)
                .AsQueryable();

            if (from.HasValue && to.HasValue)
            {
                var fromUtc = DateTime.SpecifyKind(from.Value.Date, DateTimeKind.Utc);
                var toUtc = DateTime.SpecifyKind(to.Value.Date.AddDays(1), DateTimeKind.Utc);
                query = query.Where(i => i.InspectionDate >= fromUtc && i.InspectionDate < toUtc);
            }
            else if (from.HasValue)
            {
                var start = DateTime.SpecifyKind(from.Value.Date, DateTimeKind.Utc);
                var end = start.AddDays(1);
                query = query.Where(i => i.InspectionDate >= start && i.InspectionDate < end);
            }

            if (serviceId.HasValue)
                query = query.Where(i => i.InspectionServiceId == serviceId.Value);

            var inspections = await query.OrderByDescending(i => i.Id).ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Inspections");

            worksheet.Cell(1, 1).Value = "Fecha de inspección";
            worksheet.Cell(1, 2).Value = "Vehículo";
            worksheet.Cell(1, 3).Value = "Servicio";
            worksheet.Cell(1, 4).Value = "Estatus";
            worksheet.Cell(1, 5).Value = "Departamento";
            worksheet.Cell(1, 6).Value = "Taller externo";
            worksheet.Cell(1, 7).Value = "Inspector";

            for (int i = 0; i < inspections.Count; i++)
            {
                var ins = inspections[i];
                worksheet.Cell(i + 2, 1).Value = ins.InspectionDate.ToString("yyyy-MM-dd") ?? "";
                worksheet.Cell(i + 2, 2).Value = ins.Vehicle?.LicensePlate ?? "";
                worksheet.Cell(i + 2, 3).Value = ins.InspectionService?.Name ?? "";
                worksheet.Cell(i + 2, 4).Value = ins.InspectionStatus?.Name ?? "";
                worksheet.Cell(i + 2, 5).Value = ins.Department?.Name ?? "";
                worksheet.Cell(i + 2, 6).Value = ins.ExternalWorkshopBranch?.Name ?? "";
                worksheet.Cell(i + 2, 7).Value = ins.ApplicationUser.FirstName ?? "";
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"inspections_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        [HttpGet("vehicles")]
        public async Task<IActionResult> ExportVehiclesToExcel([FromQuery] int? departmentId)
        {
            var query = _context.Vehicles
                .Include(v => v.Department)
                .Include(v => v.Brand)
                .Include(v => v.Model)
                .Include(v => v.Inspections)
                .AsQueryable();

            if (departmentId.HasValue)
                query = query.Where(v => v.DepartmentId == departmentId.Value);

            var vehicles = await query.OrderBy(v => v.Id).ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Vehicles");

            var headers = new List<string>
            {
                "Placa",
                "Número Económico",
                "Marca",
                "Modelo",
                "Año",
                "Departamento",
                "Total de inspecciones"
            };
            for (int i = 0; i < headers.Count; i++)
                worksheet.Cell(1, i + 1).Value = headers[i];

            for (int i = 0; i < vehicles.Count; i++)
            {
                var v = vehicles[i];
                var rowData = new List<string>
                {
                    v.LicensePlate ?? "",
                    v.VinNumber ?? "",
                    v.Brand?.Name ?? "",
                    v.Model?.Name ?? "",
                    v.Year?.ToString() ?? "",
                    v.Department?.Name ?? "",
                    (v.Inspections?.Count ?? 0).ToString()
                };
                for (int j = 0; j < rowData.Count; j++)
                    worksheet.Cell(i + 2, j + 1).Value = rowData[j];
            }

            worksheet.Columns().AdjustToContents();
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"vehicles_{DateTime.Now:yyyyMMdd}.xlsx");
        }
    }
}