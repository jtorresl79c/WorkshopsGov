using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkshopsGov.Data;

namespace WorkshopsGov.Controllers.Api
{
    public class CountsViewModel
    {
        public int ApplicationUsersCount { get; set; }
        public int InspectionsCount { get; set; }
        public int ExternalWorkshopsCount { get; set; }
        public int ExternalWorkshopBranchesCount { get; set; }
        public int VehiclesCount { get; set; }
    }
    
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        // GET: api/<DashboardController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // Realizar las consultas de manera secuencial
            var applicationUsersCount = await _context.ApplicationUsers.CountAsync();
            var inspectionsCount = await _context.Inspections.CountAsync();
            var externalWorkshopsCount = await _context.ExternalWorkshops.CountAsync();
            var externalWorkshopBranchesCount = await _context.ExternalWorkshopBranches.CountAsync();
            var vehiclesCount = await _context.Vehicles.CountAsync();

            var countsViewModel = new CountsViewModel
            {
                ApplicationUsersCount = applicationUsersCount,
                InspectionsCount = inspectionsCount,
                ExternalWorkshopsCount = externalWorkshopsCount,
                ExternalWorkshopBranchesCount = externalWorkshopBranchesCount,
                VehiclesCount = vehiclesCount
            };

            return Ok(countsViewModel);
        }

        // GET api/<DashboardController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<DashboardController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<DashboardController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<DashboardController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
