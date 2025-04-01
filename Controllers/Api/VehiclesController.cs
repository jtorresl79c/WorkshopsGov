using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkshopsGov.Data;

namespace WorkshopsGov.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VehiclesController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        // GET: api/<Vehicles>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<Vehicles>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<Vehicles>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<Vehicles>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<Vehicles>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        
        [HttpGet("top-vehicles")]
        public async Task<IActionResult> GetTopInspectedVehicles()
        {
            var topVehicles = await _context.Inspections
                .GroupBy(i => i.VehicleId)
                .Select(group => new
                {
                    VehicleId = group.Key,
                    Count = group.Count()
                })
                .OrderByDescending(g => g.Count)
                .Take(5)
                .Join(_context.Vehicles,
                    g => g.VehicleId,
                    v => v.Id,
                    (g, v) => new
                    {
                        v.Id,
                        v.Oficialia,
                        InspectionsCount = g.Count
                    })
                .ToListAsync();

            return Ok(topVehicles);
        }
    }
}
