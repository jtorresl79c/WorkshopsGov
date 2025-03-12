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
    public class DiagnosticPartsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiagnosticPartsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DiagnosticParts
        public async Task<IActionResult> Index()
        {
            return View(await _context.DiagnosticParts.ToListAsync());
        }

        // GET: DiagnosticParts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diagnosticPart = await _context.DiagnosticParts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (diagnosticPart == null)
            {
                return NotFound();
            }

            return View(diagnosticPart);
        }

        // GET: DiagnosticParts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DiagnosticParts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string name)
        {
            DiagnosticPart diagnosticPart = new DiagnosticPart(name) { Name = name, Active = true };
            if (ModelState.IsValid)
            {
                _context.Add(diagnosticPart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(diagnosticPart);
        }

        // GET: DiagnosticParts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diagnosticPart = await _context.DiagnosticParts.FindAsync(id);
            if (diagnosticPart == null)
            {
                return NotFound();
            }
            return View(diagnosticPart);
        }

        // POST: DiagnosticParts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Edit(int id, string name, string active)
        // public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Active,CreatedAt,UpdatedAt")] DiagnosticPart diagnosticPart)
        public async Task<IActionResult> Edit(int id, string name, bool active)
        {
            var diagnosticPart = _context.DiagnosticParts.FirstOrDefault(dp => dp.Id == id);
            if (diagnosticPart  == null)
            {
                return NotFound();
            }
            
            diagnosticPart.Name = name;
            diagnosticPart.Active = active;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(diagnosticPart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiagnosticPartExists(diagnosticPart.Id))
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
            return View(diagnosticPart);
        }

        // GET: DiagnosticParts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diagnosticPart = await _context.DiagnosticParts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (diagnosticPart == null)
            {
                return NotFound();
            }

            return View(diagnosticPart);
        }

        // POST: DiagnosticParts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var diagnosticPart = await _context.DiagnosticParts.FindAsync(id);
            if (diagnosticPart != null)
            {
                _context.DiagnosticParts.Remove(diagnosticPart);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiagnosticPartExists(int id)
        {
            return _context.DiagnosticParts.Any(e => e.Id == id);
        }
    }
}
