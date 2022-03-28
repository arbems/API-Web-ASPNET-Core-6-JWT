using Microsoft.AspNetCore.Identity;

namespace APIwebASPNETCore6JWT.Identity;

public static class AppIdentityDbContextSeed
{
    public static async Task SeedAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var scopedProvider = scope.ServiceProvider;
        try
        {
            var userManager = scopedProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scopedProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await roleManager.CreateAsync(new IdentityRole("Administrator"));

            var defaultUser = new ApplicationUser { UserName = "user@test.com", Email = "user@test.com" };
            await userManager.CreateAsync(defaultUser, "P@ss.W0rd");

            string adminUserName = "admin@test.com";
            var adminUser = new ApplicationUser { UserName = adminUserName, Email = adminUserName };
            await userManager.CreateAsync(adminUser, "P@ss.W0rd");

            adminUser = await userManager.FindByNameAsync(adminUserName);
            await userManager.AddToRoleAsync(adminUser, "Administrator");
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "An error occurred seeding the DB.");
        }
    }

}

