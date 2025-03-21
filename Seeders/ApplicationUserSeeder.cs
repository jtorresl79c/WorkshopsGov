using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkshopsGov.Models;

namespace WorkshopsGov.Seeders
{
    public static class ApplicationUserSeeder
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
        {
            var users = new List<(string Email, string FirstName, string PaternalLastName, int DepartmentId, string Role)>
            {
                ("jdoe@example.com", "John", "Doe", 1, "Verifier"),
                ("asmith@example.com", "Alice", "Smith", 2, "Administrator"),
                 ("taller@example.com", "Externo", "taller", 3, "External_Workshop")
            };

            foreach (var (email, firstName, lastName, departmentId, role) in users)
            {
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        FirstName = firstName,
                        PaternalLastName = lastName,
                        DepartmentId = departmentId,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(user, "Password123!");
                    if (result.Succeeded)
                    {
                     await userManager.AddToRoleAsync(user, role);
                    }
                }
            }
        }
    }
}