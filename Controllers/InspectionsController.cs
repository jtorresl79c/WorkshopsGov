﻿using System;
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
    public class InspectionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InspectionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Inspections
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Inspections.Include(i => i.ApplicationUser).Include(i => i.Department).Include(i => i.ExternalWorkshopBranch).Include(i => i.InspectionService).Include(i => i.InspectionStatus).Include(i => i.Vehicle);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Inspections/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspection = await _context.Inspections
                .Include(i => i.ApplicationUser)
                .Include(i => i.Department)
                .Include(i => i.ExternalWorkshopBranch)
                .Include(i => i.InspectionService)
                .Include(i => i.InspectionStatus)
                .Include(i => i.Vehicle)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inspection == null)
            {
                return NotFound();
            }

            return View(inspection);
        }

        // GET: Inspections/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name");
            ViewData["ExternalWorkshopBranchId"] = new SelectList(_context.ExternalWorkshopBranches, "Id", "Name");
            ViewData["InspectionServiceId"] = new SelectList(_context.InspectionServices, "Id", "Name");
            ViewData["InspectionStatusId"] = new SelectList(_context.InspectionStatuses, "Id", "Name");
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "Oficialia");
            return View();
        }

        // POST: Inspections/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MemoNumber,InspectionDate,CheckInTime,OperatorName,ApplicationUserId,InspectionServiceId,VehicleId,DepartmentId,ExternalWorkshopBranchId,DistanceUnit,DistanceValue,FuelLevel,FailureReport,VehicleFailureObservation,TowRequired,InspectionStatusId,Diagnostic,Active,CreatedAt,UpdatedAt")] Inspection inspection)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inspection);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", inspection.ApplicationUserId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", inspection.DepartmentId);
            ViewData["ExternalWorkshopBranchId"] = new SelectList(_context.ExternalWorkshopBranches, "Id", "Name", inspection.ExternalWorkshopBranchId);
            ViewData["InspectionServiceId"] = new SelectList(_context.InspectionServices, "Id", "Name", inspection.InspectionServiceId);
            ViewData["InspectionStatusId"] = new SelectList(_context.InspectionStatuses, "Id", "Name", inspection.InspectionStatusId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "Description", inspection.VehicleId);
            return View(inspection);
        }

        // GET: Inspections/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspection = await _context.Inspections.FindAsync(id);
            if (inspection == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", inspection.ApplicationUserId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", inspection.DepartmentId);
            ViewData["ExternalWorkshopBranchId"] = new SelectList(_context.ExternalWorkshopBranches, "Id", "Name", inspection.ExternalWorkshopBranchId);
            ViewData["InspectionServiceId"] = new SelectList(_context.InspectionServices, "Id", "Name", inspection.InspectionServiceId);
            ViewData["InspectionStatusId"] = new SelectList(_context.InspectionStatuses, "Id", "Name", inspection.InspectionStatusId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "Description", inspection.VehicleId);
            return View(inspection);
        }

        // POST: Inspections/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MemoNumber,InspectionDate,CheckInTime,OperatorName,ApplicationUserId,InspectionServiceId,VehicleId,DepartmentId,ExternalWorkshopBranchId,DistanceUnit,DistanceValue,FuelLevel,FailureReport,VehicleFailureObservation,TowRequired,InspectionStatusId,Diagnostic,Active,CreatedAt,UpdatedAt")] Inspection inspection)
        {
            if (id != inspection.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inspection);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InspectionExists(inspection.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", inspection.ApplicationUserId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", inspection.DepartmentId);
            ViewData["ExternalWorkshopBranchId"] = new SelectList(_context.ExternalWorkshopBranches, "Id", "Name", inspection.ExternalWorkshopBranchId);
            ViewData["InspectionServiceId"] = new SelectList(_context.InspectionServices, "Id", "Name", inspection.InspectionServiceId);
            ViewData["InspectionStatusId"] = new SelectList(_context.InspectionStatuses, "Id", "Name", inspection.InspectionStatusId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "Description", inspection.VehicleId);
            return View(inspection);
        }

        // GET: Inspections/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspection = await _context.Inspections
                .Include(i => i.ApplicationUser)
                .Include(i => i.Department)
                .Include(i => i.ExternalWorkshopBranch)
                .Include(i => i.InspectionService)
                .Include(i => i.InspectionStatus)
                .Include(i => i.Vehicle)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inspection == null)
            {
                return NotFound();
            }

            return View(inspection);
        }

        // POST: Inspections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inspection = await _context.Inspections.FindAsync(id);
            if (inspection != null)
            {
                _context.Inspections.Remove(inspection);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InspectionExists(int id)
        {
            return _context.Inspections.Any(e => e.Id == id);
        }
    }
}
