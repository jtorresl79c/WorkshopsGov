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
    public class DiagnosticStatusController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiagnosticStatusController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DiagnosticStatus
        public async Task<IActionResult> Index()
        {
            return View(await _context.DiagnosticStatuses.ToListAsync());
        }

        // GET: DiagnosticStatus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diagnosticStatus = await _context.DiagnosticStatuses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (diagnosticStatus == null)
            {
                return NotFound();
            }

            return View(diagnosticStatus);
        }

        // GET: DiagnosticStatus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DiagnosticStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Active")] DiagnosticStatus diagnosticStatus)
        {
            if (ModelState.IsValid)
            {
                _context.Add(diagnosticStatus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(diagnosticStatus);
        }

        // GET: DiagnosticStatus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diagnosticStatus = await _context.DiagnosticStatuses.FindAsync(id);
            if (diagnosticStatus == null)
            {
                return NotFound();
            }
            return View(diagnosticStatus);
        }

        // POST: DiagnosticStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Active")] DiagnosticStatus diagnosticStatus)
        {
            if (id != diagnosticStatus.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(diagnosticStatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiagnosticStatusExists(diagnosticStatus.Id))
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
            return View(diagnosticStatus);
        }

        // GET: DiagnosticStatus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diagnosticStatus = await _context.DiagnosticStatuses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (diagnosticStatus == null)
            {
                return NotFound();
            }

            return View(diagnosticStatus);
        }

        // POST: DiagnosticStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var diagnosticStatus = await _context.DiagnosticStatuses.FindAsync(id);
            if (diagnosticStatus != null)
            {
                _context.DiagnosticStatuses.Remove(diagnosticStatus);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiagnosticStatusExists(int id)
        {
            return _context.DiagnosticStatuses.Any(e => e.Id == id);
        }
    }
}
