using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkshopsGov.Data;

namespace WorkshopsGov.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CatalogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("inspection-services")]
        public async Task<IActionResult> GetInspectionServices()
        {
            var list = await _context.InspectionServices
                .Select(s => new { s.Id, s.Name })
                .ToListAsync();

            return Ok(list);
        }

        [HttpGet("departments")]
        public async Task<IActionResult> GetDepartments()
        {
            var list = await _context.Departments
                .Select(d => new { d.Id, d.Name })
                .ToListAsync();

            return Ok(list);
        }
    }
}