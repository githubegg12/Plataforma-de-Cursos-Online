using CursosOnline.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace CursosOnline.Web.Data
{
    public static class DbInitializer
    {
        public static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
        {
            var adminUser = await userManager.FindByEmailAsync("admin@cursosonline.com");
            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "admin@cursosonline.com",
                    Email = "admin@cursosonline.com",
                    FirstName = "Super",
                    LastName = "Admin",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user, "Admin123!");
            }
        }
    }
}
