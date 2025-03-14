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
    public class VehicleFailuresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VehicleFailuresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: VehicleFailures
        public async Task<IActionResult> Index()
        {
            return View(await _context.VehicleFailures.ToListAsync());
        }

        // GET: VehicleFailures/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicleFailure = await _context.VehicleFailures
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicleFailure == null)
            {
                return NotFound();
            }

            return View(vehicleFailure);
        }

        // GET: VehicleFailures/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: VehicleFailures/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Active,CreatedAt,UpdatedAt")] VehicleFailure vehicleFailure)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vehicleFailure);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vehicleFailure);
        }

        // GET: VehicleFailures/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicleFailure = await _context.VehicleFailures.FindAsync(id);
            if (vehicleFailure == null)
            {
                return NotFound();
            }
            return View(vehicleFailure);
        }

        // POST: VehicleFailures/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Active,CreatedAt,UpdatedAt")] VehicleFailure vehicleFailure)
        {
            if (id != vehicleFailure.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vehicleFailure);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleFailureExists(vehicleFailure.Id))
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
            return View(vehicleFailure);
        }

        // GET: VehicleFailures/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicleFailure = await _context.VehicleFailures
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicleFailure == null)
            {
                return NotFound();
            }

            return View(vehicleFailure);
        }

        // POST: VehicleFailures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehicleFailure = await _context.VehicleFailures.FindAsync(id);
            if (vehicleFailure != null)
            {
                _context.VehicleFailures.Remove(vehicleFailure);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleFailureExists(int id)
        {
            return _context.VehicleFailures.Any(e => e.Id == id);
        }
    }
}
