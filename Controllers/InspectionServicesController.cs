using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorkshopsGov.Data;
using WorkshopsGov.Models;

namespace WorkshopsGov.Controllers
{
    public class InspectionServicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InspectionServicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: InspectionServices
        public async Task<IActionResult> Index()
        {
            return View(await _context.InspectionServices.ToListAsync());
        }

        // GET: InspectionServices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspectionService = await _context.InspectionServices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inspectionService == null)
            {
                return NotFound();
            }

            return View(inspectionService);
        }

        // GET: InspectionServices/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: InspectionServices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Active,CreatedAt,UpdatedAt")] InspectionService inspectionService)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inspectionService);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(inspectionService);
        }

        // GET: InspectionServices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspectionService = await _context.InspectionServices.FindAsync(id);
            if (inspectionService == null)
            {
                return NotFound();
            }
            return View(inspectionService);
        }

        // POST: InspectionServices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Active,CreatedAt,UpdatedAt")] InspectionService inspectionService)
        {
            if (id != inspectionService.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inspectionService);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InspectionServiceExists(inspectionService.Id))
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
            return View(inspectionService);
        }

        // GET: InspectionServices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspectionService = await _context.InspectionServices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inspectionService == null)
            {
                return NotFound();
            }

            return View(inspectionService);
        }

        // POST: InspectionServices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inspectionService = await _context.InspectionServices.FindAsync(id);
            if (inspectionService != null)
            {
                _context.InspectionServices.Remove(inspectionService);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InspectionServiceExists(int id)
        {
            return _context.InspectionServices.Any(e => e.Id == id);
        }
    }
}
