using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WorkshopsGov.Data;
using WorkshopsGov.Models;

namespace WorkshopsGov.Seeders
{
    public static class WorkshopRelationsSeeder
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
        {
            var user = await userManager.FindByEmailAsync("taller@example.com");

            if (user == null) return;

            var taller = await dbContext.ExternalWorkshops
                .Include(w => w.Users)
                .FirstOrDefaultAsync(w => w.Id == 2);

            if (taller == null) return;

            // Verifica que no esté ya relacionado
            if (!taller.Users.Any(u => u.Id == user.Id))
            {
                taller.Users.Add(user);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
