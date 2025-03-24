using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkshopsGov.Data;
using WorkshopsGov.Models;
using WorkshopsGov.Models.DTOs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkshopsGov.Controllers.Api
{
    public class RoleAssignmentRequest
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
    }
    
    [Route("api/[controller]")]
    [ApiController]
    public class RolesApiController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public RolesApiController(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager
        )
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        
        // Obtener todos los roles disponibles
        [HttpGet("GetAvailableRoles")]
        public ActionResult<IEnumerable<RolDto>> GetAvailableRoles()
        {
            // var roles = _roleManager.Roles.Select(r => r.Name).ToList();
            var aspNetRoles = _roleManager.Roles.ToList();
            List<RolDto> roles = new List<RolDto>();

            foreach (var aspNetRol in aspNetRoles)
            {
                var rolDto = new RolDto();
                rolDto.Id = aspNetRol.Id ?? "";
                rolDto.Name = aspNetRol.Name ?? "";
                rolDto.NormalizedName = aspNetRol.NormalizedName ?? "";
                roles.Add(rolDto);
            }

            return roles;
        }
        
        // Obtener los roles asignados a un usuario
        [HttpGet]
        [Route("GetUserRoles/{userId}")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("Usuario no encontrado.");

            var roleNames = await _userManager.GetRolesAsync(user);
            var roles = _roleManager.Roles.Where(r => roleNames.Contains(r.Name)).ToList();

            return Ok(roles);
        }
        
        // Agregar un rol a un usuario
        [HttpPost]
        [Route("AssignRoleToUser")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] RoleAssignmentRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return NotFound("Usuario no encontrado.");

            var foundRole = await _roleManager.FindByIdAsync(request.RoleId);

            if (foundRole == null)
            {
                return BadRequest("El rol especificado no existe.");
            }
            
            var result = await _userManager.AddToRoleAsync(user, foundRole.Name);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            
            return Ok("Rol asignado exitosamente.");
        }
        
        // Quitar un rol a un usuario
        [HttpPost]
        [Route("RemoveRoleFromUser")]
        public async Task<IActionResult> RemoveRoleFromUser([FromBody] RoleAssignmentRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return NotFound("Usuario no encontrado.");
            
            var foundRole = await _roleManager.FindByIdAsync(request.RoleId);
            if (foundRole == null)
            {
                return BadRequest("El rol especificado no existe.");
            }

            if (!await _userManager.IsInRoleAsync(user, foundRole.Name))
                return BadRequest("El usuario no tiene este rol asignado.");

            var result = await _userManager.RemoveFromRoleAsync(user, foundRole.Name);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Rol removido exitosamente.");
        }
    }
}