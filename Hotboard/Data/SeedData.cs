using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Hotboard.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var provider = scope.ServiceProvider;

            var logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger("SeedData");
            var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var db = provider.GetRequiredService<ApplicationDbContext>();

            // Apply pending migrations
            await db.Database.MigrateAsync();

            string[] roles = new[] { "admin", "CSR", "shipping", "receiving", "manufacturing", "traffic", "sealshop" };

            foreach (var r in roles)
            {
                if (!await roleManager.RoleExistsAsync(r))
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole(r));
                    if (!roleResult.Succeeded)
                    {
                        var errors = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                        logger.LogError("Failed to create role '{Role}': {Errors}", r, errors);
                        throw new InvalidOperationException($"Failed to create role '{r}': {errors}");
                    }
                }
            }

            var adminUserName = "admin";
            var admin = await userManager.FindByNameAsync(adminUserName);
            if (admin == null)
            {
                admin = new IdentityUser { UserName = adminUserName, Email = "admin@local" };
                var password = "Admin123!"; // stronger password to meet default Identity policy

                var createResult = await userManager.CreateAsync(admin, password);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                    logger.LogError("Failed to create admin user: {Errors}", errors);
                    throw new InvalidOperationException($"Failed to create admin user: {errors}");
                }

                // re-fetch to ensure we have the generated Id
                admin = await userManager.FindByNameAsync(adminUserName);
            }

            if (admin == null)
            {
                throw new InvalidOperationException("Admin user was not created.");
            }

            // Safely assign admin role: check membership first, handle race where user may already be in role
            var roleName = "admin";
            var isInRole = await userManager.IsInRoleAsync(admin, roleName);
            if (!isInRole)
            {
                var addToRoleResult = await userManager.AddToRoleAsync(admin, roleName);
                if (!addToRoleResult.Succeeded)
                {
                    var errors = string.Join("; ", addToRoleResult.Errors.Select(e => e.Description));
                    // Treat 'already in role' as benign in case of race condition
                    if (errors.Contains("already in role", StringComparison.OrdinalIgnoreCase) || addToRoleResult.Errors.Any(e => e.Code?.IndexOf("AlreadyInRole", StringComparison.OrdinalIgnoreCase) >= 0))
                    {
                        logger.LogInformation("Admin user is already in role '{Role}' (race condition); continuing.", roleName);
                    }
                    else
                    {
                        logger.LogError("Failed to add admin to role: {Errors}", errors);
                        throw new InvalidOperationException($"Failed to add admin to role: {errors}");
                    }
                }
                else
                {
                    logger.LogInformation("Admin user added to role '{Role}'.", roleName);
                }
            }
            else
            {
                logger.LogInformation("Admin user already in role '{Role}'.", roleName);
            }
        }
    }
}
