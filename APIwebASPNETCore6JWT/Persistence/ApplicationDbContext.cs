using APIwebASPNETCore6JWT.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace APIwebASPNETCore6JWT.Persistence;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}

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
                Email = "user@test.com",
                FirstName = "Simón",
                LastName = "Moreno",
                UserName = "Alberto"
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