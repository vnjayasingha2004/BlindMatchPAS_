using BlindMatchPAS.Web.Authorization;
using BlindMatchPAS.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace BlindMatchPAS.Web.Data.Seed
{
    public static class AdminUserSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            var email = "admin@pas.local";
            var password = "Admin123!";

            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = "Module Leader Admin",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception("Failed to create admin user: " + errors);
                }
            }

            if (!await userManager.IsInRoleAsync(user, Roles.ModuleLeader))
            {
                await userManager.AddToRoleAsync(user, Roles.ModuleLeader);
            }
        }
    }
}
