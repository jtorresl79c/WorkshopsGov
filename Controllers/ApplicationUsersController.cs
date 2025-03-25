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
using WorkshopsGov.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace WorkshopsGov.Controllers
{
    public class ApplicationUsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ApplicationUsersController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager
        )
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: ApplicationUsers
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Users.Include(a => a.Department);
            var applicationUsers = await applicationDbContext.ToListAsync();
            
            List<ApplicationUserDto> users = new List<ApplicationUserDto>();
            
            foreach (var applicationUser in applicationUsers)
            {
                var userDto = new ApplicationUserDto();
                userDto.Id = applicationUser.Id;
                userDto.FirstName = applicationUser.FirstName;
                userDto.SecondName = applicationUser.SecondName;
                userDto.PaternalLastName = applicationUser.PaternalLastName;
                userDto.MaternalLastName = applicationUser.MaternalLastName;
                userDto.Department = applicationUser.Department.Name;
                userDto.UserName = applicationUser.UserName;
                userDto.Email = applicationUser.Email;
                userDto.PhoneNumber = applicationUser.PhoneNumber;
                userDto.CreatedAt = applicationUser.CreatedAt;
                userDto.UpdatedAt = applicationUser.UpdatedAt;
                userDto.Active = applicationUser.Active;
                users.Add(userDto);
            }
            
            return View(users);
        }

        // GET: ApplicationUsers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.Users
                .Include(a => a.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }
            
            var userDto = new ApplicationUserDto();
            userDto.Id = applicationUser.Id;
            userDto.FirstName = applicationUser.FirstName;
            userDto.SecondName = applicationUser.SecondName;
            userDto.PaternalLastName = applicationUser.PaternalLastName;
            userDto.MaternalLastName = applicationUser.MaternalLastName;
            userDto.Department = applicationUser.Department.Name;
            userDto.UserName = applicationUser.UserName;
            userDto.Email = applicationUser.Email;
            userDto.PhoneNumber = applicationUser.PhoneNumber;
            userDto.CreatedAt = applicationUser.CreatedAt;
            userDto.UpdatedAt = applicationUser.UpdatedAt;
            userDto.Active = applicationUser.Active;

            return View(userDto);
        }

        // GET: ApplicationUsers/Create
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name");
            return View();
        }

        // POST: ApplicationUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationUserDto applicationUserDto)
        {
            if (string.IsNullOrWhiteSpace(applicationUserDto.Password))
            {
                ModelState.AddModelError("Password", "El campo Contraseña es obligatorio.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name");
                return View(applicationUserDto);
            }
            
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = applicationUserDto.Email,
                    Email = applicationUserDto.Email,
                    FirstName = applicationUserDto.FirstName,
                    SecondName = applicationUserDto.SecondName,
                    PaternalLastName = applicationUserDto.PaternalLastName,
                    MaternalLastName = applicationUserDto.MaternalLastName,
                    DepartmentId = applicationUserDto.DepartmentId,
                    Active = true
                };
                
                var result = await _userManager.CreateAsync(user, applicationUserDto.Password);
                
                if (result.Succeeded)
                {
                    // User created successfully
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    // return RedirectToAction("Index", "Home");
                    TempData["SuccessMessage"] = "Los cambios se guardaron correctamente.";
                    return RedirectToAction(nameof(Index));
                }
        
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", applicationUserDto.DepartmentId);
            return View(applicationUserDto);
        }

        // GET: ApplicationUsers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {            
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUsers
                .Include(au => au.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", applicationUser.DepartmentId);
            
            var userDto = new ApplicationUserDto();
            userDto.Id = applicationUser.Id;
            userDto.FirstName = applicationUser.FirstName;
            userDto.SecondName = applicationUser.SecondName;
            userDto.PaternalLastName = applicationUser.PaternalLastName;
            userDto.MaternalLastName = applicationUser.MaternalLastName;
            userDto.Department = applicationUser.Department.Name;
            userDto.DepartmentId = applicationUser.Department.Id;
            userDto.Email = applicationUser.Email;
            userDto.PhoneNumber = applicationUser.PhoneNumber;
            userDto.CreatedAt = applicationUser.CreatedAt;
            userDto.UpdatedAt = applicationUser.UpdatedAt;
            userDto.Active = applicationUser.Active;
            
            return View(userDto);
        }

        // POST: ApplicationUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("FirstName,SecondName,PaternalLastName,MaternalLastName,DepartmentId,Active,Id,Email,PhoneNumber,Password,ConfirmPassword")] ApplicationUserDto applicationUserDto)
        {
            if (id != applicationUserDto.Id)
            {
                return NotFound();
            }
            
            var user = await _userManager.FindByIdAsync(applicationUserDto.Id);

            if (ModelState.IsValid)
            {
                try
                {
                    user.FirstName = applicationUserDto.FirstName;
                    user.SecondName = applicationUserDto.SecondName ?? "";
                    user.PaternalLastName = applicationUserDto.PaternalLastName;
                    user.MaternalLastName = applicationUserDto.MaternalLastName ?? "";
                    user.DepartmentId = applicationUserDto.DepartmentId;
                    user.PhoneNumber = applicationUserDto.PhoneNumber ?? "";

                    if (applicationUserDto.Password != null && applicationUserDto.Password != "")
                    {
                        string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                        var result = await _userManager.ResetPasswordAsync(user, resetToken, applicationUserDto.Password);
                    }

                    if (!string.Equals(user.Email, applicationUserDto.Email, StringComparison.OrdinalIgnoreCase))
                    {
                        var emailToken = await _userManager.GenerateChangeEmailTokenAsync(user, applicationUserDto.Email);
                        var emailResult = await _userManager.ChangeEmailAsync(user, applicationUserDto.Email, emailToken);
            
                        if (!emailResult.Succeeded)
                        {
                            foreach (var error in emailResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            return View(applicationUserDto);
                        }
                        
                        var usernameResult = await _userManager.SetUserNameAsync(user, applicationUserDto.Email);
            
                        if (!usernameResult.Succeeded)
                        {
                            foreach (var error in usernameResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            return View(applicationUserDto);
                        }
                    }
                    
                    var customResult = await _userManager.UpdateAsync(user);
                    
                    if (customResult.Succeeded)
                    {
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationUserExists(applicationUserDto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                // return RedirectToAction(nameof(Index));
                TempData["SuccessMessage"] = "Los cambios se guardaron correctamente.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", applicationUserDto.DepartmentId);
            return View();
        }

        // GET: ApplicationUsers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.Users
                .Include(a => a.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }
            
            var userDto = new ApplicationUserDto();
            userDto.Id = applicationUser.Id;
            userDto.FirstName = applicationUser.FirstName;
            userDto.SecondName = applicationUser.SecondName;
            userDto.PaternalLastName = applicationUser.PaternalLastName;
            userDto.MaternalLastName = applicationUser.MaternalLastName;
            userDto.Department = applicationUser.Department.Name;
            userDto.UserName = applicationUser.UserName;
            userDto.Email = applicationUser.Email;
            userDto.PhoneNumber = applicationUser.PhoneNumber;
            userDto.CreatedAt = applicationUser.CreatedAt;
            userDto.UpdatedAt = applicationUser.UpdatedAt;
            userDto.Active = applicationUser.Active;

            return View(userDto);
        }

        // POST: ApplicationUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var applicationUser = await _context.Users.FindAsync(id);
            if (applicationUser != null)
            {
                _context.Users.Remove(applicationUser);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApplicationUserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
