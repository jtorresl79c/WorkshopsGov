using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorkshopsGov.Data;
using WorkshopsGov.Models;

namespace WorkshopsGov.Controllers
{
    public class RequestServicesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public RequestServicesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: RequestServices
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.RequestServices.Include(r => r.ApplicationUser).Include(r => r.Department).Include(r => r.Vehicle);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: RequestServices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requestService = await _context.RequestServices
                .Include(r => r.ApplicationUser)
                .Include(r => r.Department)
                .Include(r => r.Vehicle)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (requestService == null)
            {
                return NotFound();
            }

            return View(requestService);
        }

        // GET: RequestServices/Create
        public IActionResult Create()
        {
            //ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name");
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "Description");
            return View();
        }

        // POST: RequestServices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Folio,ApplicationUserId,VehicleId,DepartmentId,Description,ReceptionDate")] RequestService requestService)
        {
            ModelState.Remove("Vehicle");
            ModelState.Remove("Department");

            if (ModelState.IsValid)
            {
                // Asignar el ID del usuario actual
                requestService.ApplicationUserId = _userManager.GetUserId(User);

                // Otros valores si necesitas
                requestService.Active = true;
                requestService.CreatedAt = DateTime.UtcNow;
                requestService.UpdatedAt = DateTime.UtcNow;

                _context.Add(requestService);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", requestService.DepartmentId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "Description", requestService.VehicleId);
            return View(requestService);
        }


        // GET: RequestServices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requestService = await _context.RequestServices.FindAsync(id);
            if (requestService == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", requestService.ApplicationUserId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", requestService.DepartmentId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "Description", requestService.VehicleId);
            return View(requestService);
        }

        // POST: RequestServices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Folio,ApplicationUserId,VehicleId,DepartmentId,Description,ReceptionDate,Active,CreatedAt,UpdatedAt")] RequestService requestService)
        {
            if (id != requestService.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(requestService);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequestServiceExists(requestService.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", requestService.ApplicationUserId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", requestService.DepartmentId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "Description", requestService.VehicleId);
            return View(requestService);
        }

        // GET: RequestServices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requestService = await _context.RequestServices
                .Include(r => r.ApplicationUser)
                .Include(r => r.Department)
                .Include(r => r.Vehicle)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (requestService == null)
            {
                return NotFound();
            }

            return View(requestService);
        }

        // POST: RequestServices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var requestService = await _context.RequestServices.FindAsync(id);
            if (requestService != null)
            {
                _context.RequestServices.Remove(requestService);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RequestServiceExists(int id)
        {
            return _context.RequestServices.Any(e => e.Id == id);
        }
    }
}
