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
    public class ExternalWorkshopBranchesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExternalWorkshopBranchesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🔹 Función global para obtener el taller externo y su ID de manera segura
        private async Task<bool> LoadWorkshopData(int workshopId)
        {
            var workshop = await _context.ExternalWorkshops.FindAsync(workshopId);
            if (workshop == null)
            {
                return false; // Retorna falso si no encuentra el taller
            }

            ViewBag.WorkshopName = workshop.Name;
            ViewBag.WorkshopId = workshop.Id;
            return true;
        }

        // 🔹 LISTA TODAS LAS SUCURSALES
        [HttpGet]
        [Route("ExternalWorkshopBranches/All")]
        public async Task<IActionResult> Index()
        {
            var branches = await _context.ExternalWorkshopBranches
                .Include(b => b.ExternalWorkshop)
                .ToListAsync();

            return View(branches);
        }

        // 🔹 LISTA LAS SUCURSALES FILTRADAS POR UN TALLER ESPECÍFICO
        [HttpGet]
        [Route("ExternalWorkshopBranches/Workshop/{workshopId:int}")]
        public async Task<IActionResult> Index(int workshopId)
        {
            if (!await LoadWorkshopData(workshopId))
            {
                return NotFound(); // Si no encuentra el taller, muestra error 404
            }

            var branches = await _context.ExternalWorkshopBranches
                .Where(b => b.ExternalWorkshopId == workshopId)
                .ToListAsync();

            return View("Index", branches);
        }

        // 🔹 GET: ExternalWorkshopBranches/Create
        public async Task<IActionResult> Create(int? workshopId)
        {
            if (workshopId == null || !await LoadWorkshopData(workshopId.Value))
            {
                return RedirectToAction("Index", "ExternalWorkshops"); // Redirige si no hay ID de taller
            }

            return View();
        }

        // 🔹 POST: ExternalWorkshopBranches/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Phone,Address,ExternalWorkshopId,Active,CreatedAt,UpdatedAt")] ExternalWorkshopBranch externalWorkshopBranch)

        {
            ModelState.Remove("ExternalWorkshop");

            if (ModelState.IsValid)
            {
                _context.Add(externalWorkshopBranch);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { workshopId = externalWorkshopBranch.ExternalWorkshopId });
            }

            await LoadWorkshopData(externalWorkshopBranch.ExternalWorkshopId); // Recarga el nombre del taller si hay error
            return View(externalWorkshopBranch);
        }

        // 🔹 GET: ExternalWorkshopBranches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var externalWorkshopBranch = await _context.ExternalWorkshopBranches.FindAsync(id);
            if (externalWorkshopBranch == null)
            {
                return NotFound();
            }

            if (!await LoadWorkshopData(externalWorkshopBranch.ExternalWorkshopId))
            {
                return NotFound();
            }

            return View(externalWorkshopBranch);
        }

        // 🔹 POST: ExternalWorkshopBranches/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Phone,Address,ExternalWorkshopId,Active,CreatedAt,UpdatedAt")] ExternalWorkshopBranch externalWorkshopBranch)
        {
            if (id != externalWorkshopBranch.Id)
            {
                return NotFound();
            }

            ModelState.Remove("ExternalWorkshop");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(externalWorkshopBranch);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ExternalWorkshopBranches.Any(e => e.Id == externalWorkshopBranch.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { workshopId = externalWorkshopBranch.ExternalWorkshopId });
            }

            await LoadWorkshopData(externalWorkshopBranch.ExternalWorkshopId); // Recarga el nombre del taller si hay error
            return View(externalWorkshopBranch);
        }

        // 🔹 GET: ExternalWorkshopBranches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var externalWorkshopBranch = await _context.ExternalWorkshopBranches
                .Include(e => e.ExternalWorkshop)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (externalWorkshopBranch == null)
            {
                return NotFound();
            }

            return View(externalWorkshopBranch);
        }

        // 🔹 POST: ExternalWorkshopBranches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var externalWorkshopBranch = await _context.ExternalWorkshopBranches.FindAsync(id);
            if (externalWorkshopBranch != null)
            {
                _context.ExternalWorkshopBranches.Remove(externalWorkshopBranch);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { workshopId = externalWorkshopBranch?.ExternalWorkshopId });
        }
    }
}
