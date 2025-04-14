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
            var inspectionsCount = await CountActiveInspections();
            var externalWorkshopsCount = await _context.ExternalWorkshops.CountAsync();
            var externalWorkshopBranchesCount = await _context.ExternalWorkshopBranches.CountAsync();
            var vehiclesCount = await _context.Vehicles.CountAsync();
            
            var requestsPendingDiagnosisCount = await CountRequestsPendingDiagnosis();
            var countInspectionsInRepairCount = await CountInspectionsInRepair();

            var response = new[]
            {
                new { name = "ApplicationUsersCount", count = applicationUsersCount },
                new { name = "InspectionsCount", count = inspectionsCount },
                new { name = "ExternalWorkshopsCount", count = externalWorkshopsCount },
                new { name = "ExternalWorkshopBranchesCount", count = externalWorkshopBranchesCount },
                new { name = "VehiclesCount", count = vehiclesCount },
                new { name = "RequestsPendingDiagnosisCount", count = requestsPendingDiagnosisCount },
                new { name = "CountInspectionsInRepair", count = countInspectionsInRepairCount }
            };

            return Ok(response);
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
        
        private async Task<int> CountRequestsPendingDiagnosis()
        {
            return await _context.RequestServices
                .AsNoTracking()
                .Where(rs => !rs.Inspections.Any())
                .CountAsync();
        }
        
        [HttpGet("requests-pending-diagnosis")]
        public async Task<IActionResult> RequestsPendingDiagnosis()
        {
            var total = await CountRequestsPendingDiagnosis();
            return Ok(new { requests_pending_diagnosis = total });
        }
        
        private async Task<int> CountActiveInspections()
        {
            return await _context.Inspections
                .AsNoTracking()
                .Where(i => i.Active)
                .CountAsync();
        }

        [HttpGet("active-inspections")]
        public async Task<IActionResult> ActiveInspections()
        {
            var total = await CountActiveInspections();
            return Ok(new { active_inspections = total });
        }
        
        private async Task<int> CountInspectionsWithAssignedWorkshop()
        {
            return await _context.Inspections
                .AsNoTracking()
                .Where(i => i.Active && i.ExternalWorkshopBranchId != 1)
                .CountAsync();
        }

        [HttpGet("inspections-with-assigned-workshop")]
        public async Task<IActionResult> InspectionsWithAssignedWorkshop()
        {
            var total = await CountInspectionsWithAssignedWorkshop();
            return Ok(new { inspections_with_assigned_workshop = total });
        }
        
        private async Task<int> CountInspectionsWithPendingQuote()
        {
            return await _context.Inspections
                .AsNoTracking()
                .Where(i => i.Active && i.InspectionStatusId == 4)
                .CountAsync();
        }

        [HttpGet("inspections-with-pending-quote")]
        public async Task<IActionResult> InspectionsWithPendingQuote()
        {
            var total = await CountInspectionsWithPendingQuote();
            return Ok(new { inspections_with_pending_quote = total });
        }
        
        private async Task<int> CountInspectionsInRepair()
        {
            return await _context.Inspections
                .AsNoTracking()
                .Where(i => i.Active && i.InspectionStatusId == 5)
                .CountAsync();
        }

        [HttpGet("inspections-in-repair")]
        public async Task<IActionResult> InspectionsInRepair()
        {
            var total = await CountInspectionsInRepair();
            return Ok(new { inspections_in_repair = total });
        }

    }
}
