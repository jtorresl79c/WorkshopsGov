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
            var workshops = await _context.ExternalWorkshops
                .Where(workshop => workshop.Id != 1) // 🔥 Excluir el ID 1
                .Select(workshop => new
                {
                    workshop.Id,
                    workshop.Name,
                    workshop.Active,
                    BranchCount = _context.ExternalWorkshopBranches.Count(b => b.ExternalWorkshopId == workshop.Id)
                })
                .ToListAsync();

            ViewBag.Workshops = workshops;
            return View();
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExternalWorkshop externalWorkshop)
        {
            // Leer datos del usuario desde el formulario
            string email = Request.Form["UserEmail"];
            string password = Request.Form["UserPassword"];
            string confirmPassword = Request.Form["ConfirmPassword"];

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Debe ingresar el correo y la contraseña para el usuario del taller.");
                return View(externalWorkshop);
            }

            if (password != confirmPassword)
            {
                ModelState.AddModelError("", "Las contraseñas no coinciden.");
                return View(externalWorkshop);
            }

            if (ModelState.IsValid)
            {
                // Guardar el taller
                externalWorkshop.CreatedAt = DateTime.UtcNow;
                externalWorkshop.UpdatedAt = DateTime.UtcNow;
                externalWorkshop.Active = true;

                _context.Add(externalWorkshop);
                await _context.SaveChangesAsync();

                // Crear el usuario
                var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();

                var user = new ApplicationUser
                {
                    Email = email,
                    UserName = email,
                    FirstName = "Taller",
                    PaternalLastName = "Externo",
                    EmailConfirmed = true,
                    Active = true,
                    DepartmentId = 3
                };

                var userResult = await userManager.CreateAsync(user, password);
                if (userResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "External_Workshop");

                    // Agregar manualmente a la tabla intermedia external_workshop_user
                    _context.Set<Dictionary<string, object>>("external_workshop_user").Add(
                          new Dictionary<string, object>
                          {
                              ["UserId"] = user.Id,
                              ["ExternalWorkshopId"] = externalWorkshop.Id
                          }
                      );

                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in userResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return View(externalWorkshop);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Name,Active,CreatedAt,UpdatedAt")] ExternalWorkshop externalWorkshop)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(externalWorkshop);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(externalWorkshop);
        //}

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

            // Buscar usuario asignado a este taller (relación many-to-many)
            var userId = await _context.Set<Dictionary<string, object>>("external_workshop_user")
            .Where(x => (int)x["ExternalWorkshopId"] == id)
            .Select(x => (string)x["UserId"])
            .FirstOrDefaultAsync();

            if (userId != null)
            {
                var user = await _context.Users.FindAsync(userId);
                ViewBag.AssignedUser = user;
            }

            return View(externalWorkshop);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Active,CreatedAt,UpdatedAt")] ExternalWorkshop externalWorkshop)
        {
            if (id != externalWorkshop.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(externalWorkshop);
            }

            try
            {
                // Actualiza taller
                externalWorkshop.UpdatedAt = DateTime.UtcNow;
                _context.Update(externalWorkshop);

                // Buscar el usuario asignado al taller (desde tabla intermedia)
                var userId = await _context.Set<Dictionary<string, object>>("external_workshop_user")
                    .Where(x => (int)x["ExternalWorkshopId"] == externalWorkshop.Id)
                    .Select(x => (string)x["UserId"])
                    .FirstOrDefaultAsync();

                var newEmail = Request.Form["UserEmail"];
                var newPassword = Request.Form["NewPassword"];

                if (!string.IsNullOrWhiteSpace(userId))
                {
                    var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
                    var user = await userManager.FindByIdAsync(userId);

                    if (user != null)
                    {
                        // Solo actualiza si hay cambios
                        var hasChanges = false;

                        // Verifica si el correo fue modificado
                        if (!string.IsNullOrWhiteSpace(newEmail) && user.Email != newEmail)
                        {
                            user.Email = newEmail;
                            user.UserName = newEmail;
                            hasChanges = true;
                        }

                        // Si hay cambios en email, intenta actualizar
                        if (hasChanges)
                        {
                            var result = await userManager.UpdateAsync(user);
                            if (!result.Succeeded)
                            {
                                foreach (var error in result.Errors)
                                    ModelState.AddModelError("", error.Description);
                                return View(externalWorkshop);
                            }
                        }

                        // Si se proporcionó nueva contraseña
                        if (!string.IsNullOrWhiteSpace(newPassword))
                        {
                            var token = await userManager.GeneratePasswordResetTokenAsync(user);
                            var passResult = await userManager.ResetPasswordAsync(user, token, newPassword);

                            if (!passResult.Succeeded)
                            {
                                foreach (var error in passResult.Errors)
                                    ModelState.AddModelError("", error.Description);
                                return View(externalWorkshop);
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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
