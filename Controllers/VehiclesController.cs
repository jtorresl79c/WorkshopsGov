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
    public class VehiclesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VehiclesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Vehicles
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Vehicles.Include(v => v.Brand).Include(v => v.Department).Include(v => v.Model).Include(v => v.Sector).Include(v => v.VehicleStatus).Include(v => v.VehicleType);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Vehicles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.Department)
                .Include(v => v.Model)
                .Include(v => v.Sector)
                .Include(v => v.VehicleStatus)
                .Include(v => v.VehicleType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // GET: Vehicles/Create
        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name");
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name");
            ViewData["ModelId"] = new SelectList(_context.VehicleModels, "Id", "Name");
            ViewData["SectorId"] = new SelectList(_context.Sectors, "Id", "Name");
            ViewData["VehicleStatusId"] = new SelectList(_context.VehicleStatuses, "Id", "Name");
            ViewData["VehicleTypeId"] = new SelectList(_context.VehicleTypes, "Id", "Name");
            return View();
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Oficialia,LicensePlate,VinNumber,Description,DepartmentId,VehicleStatusId,Year,BrandId,ModelId,Engine,SectorId,VehicleTypeId,Active")] Vehicle vehicle)
        {
            if (true) // ModelState.IsValid
            {
                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", vehicle.BrandId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", vehicle.DepartmentId);
            ViewData["ModelId"] = new SelectList(_context.VehicleModels, "Id", "Name", vehicle.ModelId);
            ViewData["SectorId"] = new SelectList(_context.Sectors, "Id", "Name", vehicle.SectorId);
            ViewData["VehicleStatusId"] = new SelectList(_context.VehicleStatuses, "Id", "Name", vehicle.VehicleStatusId);
            ViewData["VehicleTypeId"] = new SelectList(_context.VehicleTypes, "Id", "Name", vehicle.VehicleTypeId);
            return View(vehicle);
        }

        // GET: Vehicles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", vehicle.BrandId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", vehicle.DepartmentId);
            ViewData["ModelId"] = new SelectList(_context.VehicleModels, "Id", "Name", vehicle.ModelId);
            ViewData["SectorId"] = new SelectList(_context.Sectors, "Id", "Name", vehicle.SectorId);
            ViewData["VehicleStatusId"] = new SelectList(_context.VehicleStatuses, "Id", "Name", vehicle.VehicleStatusId);
            ViewData["VehicleTypeId"] = new SelectList(_context.VehicleTypes, "Id", "Name", vehicle.VehicleTypeId);
            return View(vehicle);
        }

        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Oficialia,LicensePlate,VinNumber,Description,DepartmentId,VehicleStatusId,Year,BrandId,ModelId,Engine,SectorId,VehicleTypeId,Active")] Vehicle vehicle)
        {
            if (id != vehicle.Id)
            {
                return NotFound();
            }

            if (true) // ModelState.IsValid
            {
                try
                {
                    _context.Update(vehicle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicle.Id))
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
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", vehicle.BrandId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", vehicle.DepartmentId);
            ViewData["ModelId"] = new SelectList(_context.VehicleModels, "Id", "Name", vehicle.ModelId);
            ViewData["SectorId"] = new SelectList(_context.Sectors, "Id", "Name", vehicle.SectorId);
            ViewData["VehicleStatusId"] = new SelectList(_context.VehicleStatuses, "Id", "Name", vehicle.VehicleStatusId);
            ViewData["VehicleTypeId"] = new SelectList(_context.VehicleTypes, "Id", "Name", vehicle.VehicleTypeId);
            return View(vehicle);
        }

        // GET: Vehicles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.Department)
                .Include(v => v.Model)
                .Include(v => v.Sector)
                .Include(v => v.VehicleStatus)
                .Include(v => v.VehicleType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(int id)
        {
            return _context.Vehicles.Any(e => e.Id == id);
        }
    }
}
