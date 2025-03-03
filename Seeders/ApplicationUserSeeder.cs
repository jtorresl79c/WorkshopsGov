using WorkshopsGov.Models;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using WorkshopsGov.Data;

namespace WorkshopsGov.Seeders
{
    public static class ApplicationUserSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Users.Any())
            {
                var users = new ApplicationUser[]
                {
                    new ApplicationUser
                    {
                        UserName = "jdoe@example.com",
                        NormalizedUserName = "JDOE@EXAMPLE.COM",
                        Email = "jdoe@example.com",
                        NormalizedEmail = "JDOE@EXAMPLE.COM",
                        FirstName = "John",
                        SecondName = "",
                        PaternalLastName = "Doe",
                        MaternalLastName = "",
                        DepartmentId = 1,
                        EmailConfirmed = true,
                        PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(new ApplicationUser(), "Password123!")
                    },
                    new ApplicationUser
                    {
                        UserName = "asmith@example.com",
                        NormalizedUserName = "ASMITH@EXAMPLE.COM",
                        Email = "asmith@example.com",
                        NormalizedEmail = "ASMITH@EXAMPLE.COM",
                        FirstName = "Alice",
                        SecondName = "",
                        PaternalLastName = "Smith",
                        MaternalLastName = "",
                        DepartmentId = 2,
                        EmailConfirmed = true,
                        PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(new ApplicationUser(), "Password123!")
                    }
                };

                foreach (var user in users)
                {
                    context.Users.Add(user);
                }
                context.SaveChanges();
            }
        }
    }
}