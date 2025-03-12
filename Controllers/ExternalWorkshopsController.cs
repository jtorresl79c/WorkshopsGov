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
    public class ExternalWorkshopsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExternalWorkshopsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ExternalWorkshops
        public async Task<IActionResult> Index()
        {
            return View(await _context.ExternalWorkshops.ToListAsync());
        }

        // GET: ExternalWorkshops/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var externalWorkshop = await _context.ExternalWorkshops
                .FirstOrDefaultAsync(m => m.Id == id);
            if (externalWorkshop == null)
            {
                return NotFound();
            }

            return View(externalWorkshop);
        }

        // GET: ExternalWorkshops/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ExternalWorkshops/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Active,CreatedAt,UpdatedAt")] ExternalWorkshop externalWorkshop)
        {
            if (ModelState.IsValid)
            {
                _context.Add(externalWorkshop);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(externalWorkshop);
        }

        // GET: ExternalWorkshops/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var externalWorkshop = await _context.ExternalWorkshops.FindAsync(id);
            if (externalWorkshop == null)
            {
                return NotFound();
            }
            return View(externalWorkshop);
        }

        // POST: ExternalWorkshops/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Active,CreatedAt,UpdatedAt")] ExternalWorkshop externalWorkshop)
        {
            if (id != externalWorkshop.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(externalWorkshop);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExternalWorkshopExists(externalWorkshop.Id))
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
            return View(externalWorkshop);
        }

        // GET: ExternalWorkshops/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var externalWorkshop = await _context.ExternalWorkshops
                .FirstOrDefaultAsync(m => m.Id == id);
            if (externalWorkshop == null)
            {
                return NotFound();
            }

            return View(externalWorkshop);
        }

        // POST: ExternalWorkshops/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var externalWorkshop = await _context.ExternalWorkshops.FindAsync(id);
            if (externalWorkshop != null)
            {
                _context.ExternalWorkshops.Remove(externalWorkshop);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExternalWorkshopExists(int id)
        {
            return _context.ExternalWorkshops.Any(e => e.Id == id);
        }
    }
}
