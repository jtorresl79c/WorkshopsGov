using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WorkshopsGov.Data;
using WorkshopsGov.Models;

namespace WorkshopsGov.Controllers
{
    public class WorkshopQuotesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkshopQuotesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WorkshopQuotes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.WorkshopQuote.Include(w => w.Inspection).Include(w => w.WorkshopBranch);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: WorkshopQuotes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workshopQuote = await _context.WorkshopQuote
                .Include(w => w.Inspection)
                .Include(w => w.WorkshopBranch)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workshopQuote == null)
            {
                return NotFound();
            }

            return View(workshopQuote);
        }

        // GET: WorkshopQuotes/Create
        //public IActionResult Create()
        //{
        //    ViewData["InspectionId"] = new SelectList(_context.Inspections, "Id", "ApplicationUserId");
        //    ViewData["WorkshopBranchId"] = new SelectList(_context.ExternalWorkshopBranches, "Id", "Name");
        //    return View();
        //}
        public async Task<IActionResult> Create(int inspectionId)
        {
            var userName = User.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
                return RedirectToAction("Login", "Account");

            var user = await _context.Users
                .Include(u => u.ExternalWorkshops)
                    .ThenInclude(ew => ew.ExternalWorkshopBranches)
                .FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
                return BadRequest("Usuario no encontrado.");

            // Taller asignado al usuario (evita el ID 1)
            var tallerAsignado = user.ExternalWorkshops
                .FirstOrDefault(ew => ew.Id != 1);

            if (tallerAsignado == null)
                return BadRequest("No tiene un taller asignado válido.");

            // Obtener la inspección (para preseleccionar sucursal)
            var inspection = await _context.Inspections
                .Include(i => i.ExternalWorkshopBranch)
                .FirstOrDefaultAsync(i => i.Id == inspectionId);

            if (inspection == null)
                return NotFound("Inspección no encontrada.");

            // Sucursales disponibles para el taller asignado
            var branches = tallerAsignado.ExternalWorkshopBranches;

            ViewData["WorkshopBranchId"] = new SelectList(
                branches, "Id", "Name", inspection.ExternalWorkshopBranchId // ← preselección aquí
            );

            ViewBag.WorkshopInfo = new
            {
                WorkshopId = tallerAsignado.Id,
                WorkshopName = tallerAsignado.Name
            };

            ViewBag.InspectionId = inspectionId;

            var model = new WorkshopQuote
            {
                InspectionId = inspectionId,
                QuoteDate = DateTime.Now.Date,
                EstimatedCompletionDate = DateTime.Now.Date,
                WorkshopBranchId = inspection.ExternalWorkshopBranchId
            };

            return View(model);
        }



        // POST: WorkshopQuotes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,InspectionId,WorkshopBranchId,QuoteNumber,QuoteDate,TotalCost,EstimatedCompletionDate,QuoteDetails")] WorkshopQuote workshopQuote)
        {
            ModelState.Remove("Inspection");
            ModelState.Remove("WorkshopBranch");
            ModelState.Remove("QuoteStatus");

            if (ModelState.IsValid)
            {
                workshopQuote.QuoteStatusId = 1; // Capturada por defecto
                workshopQuote.QuoteDate = DateTime.SpecifyKind(workshopQuote.QuoteDate, DateTimeKind.Utc);
                workshopQuote.EstimatedCompletionDate = DateTime.SpecifyKind(workshopQuote.EstimatedCompletionDate, DateTimeKind.Utc);
                _context.Add(workshopQuote);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Inspections", new { id = workshopQuote.InspectionId });
                //return RedirectToAction(nameof(Index));
            }


            ViewData["WorkshopBranchId"] = new SelectList(_context.ExternalWorkshopBranches, "Id", "Name");
            return View(workshopQuote);
        }


        // GET: WorkshopQuotes/Edit/5
        public async Task<IActionResult> Edit(int? id, int? inspectionId)
        {
            // GET: WorkshopQuotes/Edit/5

            if (id == null)
                return NotFound();

            var workshopQuote = await _context.WorkshopQuote
                .Include(w => w.Inspection)
                .Include(w => w.WorkshopBranch)
                .ThenInclude(b => b.ExternalWorkshop)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (workshopQuote == null)
                return NotFound();

            var userName = User.Identity?.Name;
            var user = await _context.Users
                .Include(u => u.ExternalWorkshops)
                .ThenInclude(ew => ew.ExternalWorkshopBranches)
                .FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
                return BadRequest("Usuario no válido");

            var tallerAsignado = user.ExternalWorkshops.FirstOrDefault(ew => ew.Id != 1);
            if (tallerAsignado == null)
                return BadRequest("Taller no válido");

            var branches = tallerAsignado.ExternalWorkshopBranches;

            ViewData["WorkshopBranchId"] = new SelectList(branches, "Id", "Name", workshopQuote.WorkshopBranchId);
            ViewBag.InspectionId = workshopQuote.InspectionId;

            ViewBag.WorkshopInfo = new
            {
                WorkshopId = tallerAsignado.Id,
                WorkshopName = tallerAsignado.Name
            };

            return View(workshopQuote);

            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var workshopQuote = await _context.WorkshopQuote.FindAsync(id);
            //if (workshopQuote == null)
            //{
            //    return NotFound();
            //}
            //ViewData["InspectionId"] = new SelectList(_context.Inspections, "Id", "ApplicationUserId", workshopQuote.InspectionId);
            //ViewData["WorkshopBranchId"] = new SelectList(_context.ExternalWorkshopBranches, "Id", "Name", workshopQuote.WorkshopBranchId);
            //return View(workshopQuote);
        }

        // POST: WorkshopQuotes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,InspectionId,WorkshopBranchId,QuoteNumber,QuoteDate,TotalCost,EstimatedCompletionDate,QuoteDetails")] WorkshopQuote workshopQuote)
        {
            ModelState.Remove("Inspection");
            ModelState.Remove("WorkshopBranch");
            ModelState.Remove("QuoteStatus");

            if (id != workshopQuote.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.WorkshopQuote.FindAsync(id);
                    if (existing == null)
                        return NotFound();

                    existing.QuoteNumber = workshopQuote.QuoteNumber;
                    existing.QuoteDate = DateTime.SpecifyKind(workshopQuote.QuoteDate, DateTimeKind.Utc);
                    existing.EstimatedCompletionDate = DateTime.SpecifyKind(workshopQuote.EstimatedCompletionDate, DateTimeKind.Utc);
                    existing.TotalCost = workshopQuote.TotalCost;
                    existing.QuoteDetails = workshopQuote.QuoteDetails;
                    existing.WorkshopBranchId = workshopQuote.WorkshopBranchId;

                    // Ya está siendo trackeado, solo guarda
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Details", "Inspections", new { id = existing.InspectionId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkshopQuoteExists(workshopQuote.Id))
                        return NotFound();
                    else
                        throw;
                }
            }

            ViewData["WorkshopBranchId"] = new SelectList(_context.ExternalWorkshopBranches, "Id", "Name", workshopQuote.WorkshopBranchId);
            return View(workshopQuote);
        }


        // GET: WorkshopQuotes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workshopQuote = await _context.WorkshopQuote
                .Include(w => w.Inspection)
                .Include(w => w.WorkshopBranch)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workshopQuote == null)
            {
                return NotFound();
            }

            return View(workshopQuote);
        }

        // POST: WorkshopQuotes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workshopQuote = await _context.WorkshopQuote.FindAsync(id);
            if (workshopQuote != null)
            {
                _context.WorkshopQuote.Remove(workshopQuote);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkshopQuoteExists(int id)
        {
            return _context.WorkshopQuote.Any(e => e.Id == id);
        }
    }
}
