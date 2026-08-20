using Microsoft.AspNetCore.Identity;
using MovieApi.Enums;
using MovieApi.Models.Identity;

namespace MovieApi.Data.Seed;

public static class IdentitySeedDataExtensions
{
    private const string AdminRole = "Admin";
    private const string UserRole = "User";

    public static async Task SeedIdentityAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();

        RoleManager<IdentityRole<Guid>> roleManager =
            scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        UserManager<ApplicationUser> userManager =
            scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await CreateRoleAsync(roleManager, AdminRole);
        await CreateRoleAsync(roleManager, UserRole);

        await CreateUserAsync(
            userManager,
            "admin@example.com",
            "Admin123!",
            [AdminRole, UserRole]
        );

        await CreateUserAsync(
            userManager,
            "user@example.com",
            "User123!",
            [UserRole]
        );
    }

    private static async Task CreateRoleAsync(
        RoleManager<IdentityRole<Guid>> roleManager,
        string roleName)
    {
        if (await roleManager.RoleExistsAsync(roleName))
        {
            return;
        }

        IdentityResult result = await roleManager.CreateAsync(
            new IdentityRole<Guid>
            {
                Id = Guid.CreateVersion7(),
                Name = roleName
            }
        );

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(
                    ", ",
                    result.Errors.Select(error => error.Description)
                )
            );
        }
    }

    private static async Task CreateUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        IEnumerable<string> roles)
    {
        ApplicationUser? existingUser =
            await userManager.FindByEmailAsync(email);

        if (existingUser is not null)
        {
            await AddRolesAsync(userManager, existingUser, roles);
            return;
        }

        DateTime now = DateTime.UtcNow;

        ApplicationUser user = new()
        {
            Id = Guid.CreateVersion7(),
            Email = email,
            UserName = email,
            EmailConfirmed = true,
            Status = UserStatus.Active,
            CreatedAt = now,
            UpdatedAt = now
        };

        IdentityResult createResult =
            await userManager.CreateAsync(user, password);

        if (!createResult.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(
                    ", ",
                    createResult.Errors.Select(error => error.Description)
                )
            );
        }

        await AddRolesAsync(userManager, user, roles);
    }

    private static async Task AddRolesAsync(
        UserManager<ApplicationUser> userManager,
        ApplicationUser user,
        IEnumerable<string> roles)
    {
        foreach (string role in roles)
        {
            if (await userManager.IsInRoleAsync(user, role))
            {
                continue;
            }

            IdentityResult result =
                await userManager.AddToRoleAsync(user, role);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join(
                        ", ",
                        result.Errors.Select(error => error.Description)
                    )
                );
            }
        }
    }
}
