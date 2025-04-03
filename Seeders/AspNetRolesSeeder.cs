using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WorkshopsGov.Seeders
{
    public static class AspNetRolesSeeder
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {
            var roles = new List<string>
            {
                "Verifier",
                "Municipal_Workshop",
                "External_Workshop",
                "Sector_Global",
                "Administrator"
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}