using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using learnify.ai.api.Common.Infrastructure.Data;
using learnify.ai.api.Domain.Entities;

namespace learnify.ai.api.Common.Infrastructure.Data;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LearnifyDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

        // Ensure database created / migrated
        await context.Database.EnsureCreatedAsync();

        // Seed roles
        var roles = new[] { "Admin", "Instructor", "Student" };
        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new Role(roleName));
            }
        }

        // Seed default admin if not exists
        var adminEmail = "admin@learnify.com";
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            var createResult = await userManager.CreateAsync(admin, "Admin@123");
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
