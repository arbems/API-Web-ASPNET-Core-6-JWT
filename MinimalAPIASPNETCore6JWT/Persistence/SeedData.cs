using MinimalAPIASPNETCore6JWT.Entities;
using Microsoft.AspNetCore.Identity;

namespace MinimalAPIASPNETCore6JWT.Persistence;

public static class SeedData
{
	public static async Task Inicialize(WebApplication app)
    {
        var scopeFactory = app!.Services.GetRequiredService<IServiceScopeFactory>();
        using var scope = scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        context.Database.EnsureCreated();

        if (!userManager.Users.Any())
        {
            logger.LogInformation("Creando usuario de prueba");

            var newUser = new User
            {
                Email = "alberto@test.com",
                FirstName = "moreno",
                LastName = "simon",
                UserName = "alberto"
            };

            await userManager.CreateAsync(newUser, "P@ss.W0rd");
            await roleManager.CreateAsync(new IdentityRole
            {
                Name = "Admin"
            });
            await roleManager.CreateAsync(new IdentityRole
            {
                Name = "AnotherRole"
            });

            await userManager.AddToRoleAsync(newUser, "Admin");
            await userManager.AddToRoleAsync(newUser, "AnotherRole");
        }
    }
}

