using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Pawfectmatch_V._1.Models; 

namespace Pawfectmatch_V._1.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            var adminUser = await userManager.FindByEmailAsync("admin@pawfectmatch.com");
            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "admin@pawfectmatch.com",
                    Email = "admin@pawfectmatch.com",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, "Admin#1234");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
}
