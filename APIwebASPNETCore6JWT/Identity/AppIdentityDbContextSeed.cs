using Microsoft.AspNetCore.Identity;
using System;

namespace APIwebASPNETCore6JWT.Identity;

public static class AppIdentityDbContextSeed
{
    #region Public Methods

    public static async Task SeedAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var scopedProvider = scope.ServiceProvider;
        try
        {
            var userManager = scopedProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scopedProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var roles = GetListOfRolesToSeed();
            var users = GetListOfUsersToSeed();

            // 1. ASP.NET Core Identity: Add roles.
            foreach (var role in roles)
            {
                // Asp Net Identity: Create roles to assign user after.
                var createdRole = await roleManager.CreateAsync(new IdentityRole(role.RoleName));

                if (!createdRole.Succeeded)
                {
                    throw new Exception($"role {role.RoleName} not created.");
                }
            }

            // 2. ASP.NET Core Identity: Add users and assigning roles.
            foreach (var user in users)
            {
                var applicationUser = new ApplicationUser()
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                };

                // ASP.NET Core Identity: Create user.
                var createUserTaskResult = await userManager.CreateAsync(applicationUser, user.Password);

                if (!createUserTaskResult.Succeeded)
                {
                    throw new Exception($"User {applicationUser.UserName} not created.");
                }

                // ASP.NET Core Identity: Finding user to assign role.
                var foundUser = await userManager.FindByNameAsync(user.UserName);

                if (foundUser == null)
                {
                    throw new Exception($"User not created.");
                }

                // ASP.NET Core Identity: Assigning role to user.
                var rolledUser = await userManager.AddToRoleAsync(applicationUser, user.Role);

                if (!rolledUser.Succeeded)
                {
                    throw new Exception($"{user.UserName} is not enrolled.");
                }
            }
        }
        catch (Exception ex)
        {
            app.Logger.LogError($"An error occurred seeding the DB: {exception.Message}");
        }
    }

    #endregion

    #region Private Methods

    private static IList<UserToSeed> GetListOfUsersToSeed()
    {
        return new List<UserToSeed>()
            {
                new UserToSeed()
                {
                    UserName = "javi.karra",
                    Password = "P@ss.W0rd@javi",
                    Email = "javi.karra@mycompany.com",
                    PhoneNumber = "1234567890",
                    Role = RoleDefinition.USER_ROLE,
                },
                new UserToSeed()
                {
                    UserName = "lucas.perez",
                    Password = "P@ss.W0rd@lucas",
                    Email = "lucas.perez@mycompany.com",
                    PhoneNumber = "6234567890",
                    Role = RoleDefinition.USER_ROLE,
                },
                new UserToSeed()
                {
                    UserName = "admin",
                    Password = "P@ss.W0rd@admin",
                    Email = "admin@test.com",
                    PhoneNumber = "623455699",
                    Role = RoleDefinition.ADMINISTRATOR_ROLE,
                }
            };
    }

    private static IList<RolesToSeed> GetListOfRolesToSeed()
    {
        return new List<RolesToSeed>()
            {
                new RolesToSeed() { RoleName = RoleDefinition.ADMINISTRATOR_ROLE },
                new RolesToSeed() { RoleName = RoleDefinition.USER_ROLE }
            };
    }

    #endregion
}