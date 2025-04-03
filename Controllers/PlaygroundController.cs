using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkshopsGov.Data;
using System.Threading.Tasks;

namespace WorkshopsGov.Controllers;

// RequestServiceTestDto deberia estar en Models/DTOs, pero como es de prueba lo dejare aqui
public class RequestServiceTestDto
{
    public int Id { get; set; }
    public string Folio { get; set; } = string.Empty;
    public string VehicleInfo { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public List<string> InspectionMemos { get; set; } = new();
    public List<string> FileNames { get; set; } = new();
}

public class PlaygroundController : Controller
{
    private readonly ApplicationDbContext _context;

    public PlaygroundController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [Route("api/[controller]/Get")]
    public IEnumerable<string> Get()
    {
        return new string[] { "Get Playground API 1", "Get Playground API 2" };
    }

    [HttpPost]
    [Route("api/[controller]/Create")]
    public IEnumerable<string> Create()
    {
        return new string[] { "Post Playground API 1", "Post Playground API 2" };
    }

    [HttpGet]
    [Route("api/[controller]/test-request-services")]
    public async Task<IActionResult> TestRequestServices()
    {
        var data = await _context.RequestServices
            .Include(rs => rs.Vehicle)
            .ThenInclude(v => v.Brand)
            .Include(rs => rs.Department)
            .Include(rs => rs.Inspections)
            .Include(rs => rs.Files)
            .Select(rs => new RequestServiceTestDto
            {
                Id = rs.Id,
                Folio = rs.Folio,
                VehicleInfo = $"{rs.Vehicle.LicensePlate} ({rs.Vehicle.Brand.Name})",
                DepartmentName = rs.Department.Name,
                InspectionMemos = rs.Inspections.Select(i => i.MemoNumber).ToList(),
                FileNames = rs.Files.Select(f => f.Name).ToList()
            })
            .ToListAsync();

        return Ok(data);
    }
}