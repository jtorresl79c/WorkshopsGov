using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkshopsGov.Data;
using WorkshopsGov.Models;
using WorkshopsGov.Models.DTOs;

namespace WorkshopsGov.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUsersApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ApplicationUsersApiController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        // GET: api/<ApplicationUsersApiController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationUserDto>>> GetAll()
        {
            var applicationUsers = await _context.ApplicationUsers.Include(au => au.Department).ToListAsync();
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

            return users;
        }

        // GET api/<ApplicationUsersApiController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ApplicationUsersApiController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ApplicationUsersApiController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ApplicationUsersApiController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
