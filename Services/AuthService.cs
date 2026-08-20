using Microsoft.AspNetCore.Identity;
using MovieApi.DTOs.Auth;
using MovieApi.Enums;
using MovieApi.Interfaces.Service;
using MovieApi.Models.Identity;

namespace MovieApi.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ApplicationUser?> AuthenticateAsync(LoginDto loginDto)
    {
        ApplicationUser? user =
            await _userManager.FindByEmailAsync(loginDto.Email);

        if (user is null)
        {
            return null;
        }

        if (user.Status != UserStatus.Active)
        {
            return null;
        }

        bool passwordIsValid =
            await _userManager.CheckPasswordAsync(
                user,
                loginDto.Password
            );

        if (!passwordIsValid)
        {
            return null;
        }

        return user;
    }

    public async Task<IList<string>> GetRolesAsync(
        ApplicationUser user)
    {
        return await _userManager.GetRolesAsync(user);
    }

    public async Task<ApplicationUser?> FindActiveUserByIdAsync(
    Guid userId)
    {
        ApplicationUser? user =
            await _userManager.FindByIdAsync(
                userId.ToString()
            );

        if (user is null)
        {
            return null;
        }

        if (user.Status != UserStatus.Active)
        {
            return null;
        }

        return user;
    }
}
