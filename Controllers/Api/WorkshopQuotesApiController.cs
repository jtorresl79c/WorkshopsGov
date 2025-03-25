using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkshopsGov.Data;

namespace WorkshopsGov.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkshopQuotesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WorkshopQuotesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/workshopquotes/by-inspection/1
        [HttpGet("by-inspection/{inspectionId}")]
        public async Task<IActionResult> GetQuotesByInspection(int inspectionId)
        {
            var quotes = await _context.WorkshopQuote
                .Where(q => q.InspectionId == inspectionId)
                .Include(q => q.WorkshopBranch)
                    .ThenInclude(b => b.ExternalWorkshop)
                .Include(q => q.QuoteStatus)
                .OrderByDescending(q => q.QuoteDate)
                .Select(q => new
                {
                    q.Id,
                    q.QuoteNumber,
                    q.QuoteDate,
                    q.TotalCost,
                    q.EstimatedCompletionDate,
                    q.QuoteDetails,
                    QuoteStatus = q.QuoteStatus.Name,
                    WorkshopBranch = q.WorkshopBranch.Name,
                    WorkshopName = q.WorkshopBranch.ExternalWorkshop.Name
                })
                .ToListAsync();

            return Ok(quotes);
        }
    }
}
