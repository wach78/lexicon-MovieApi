using MovieApi.DTOs.Auth;
using MovieApi.Models.Identity;

namespace MovieApi.Interfaces.Service;

public interface IAuthService
{
    Task<ApplicationUser?> AuthenticateAsync(LoginDto loginDto);

    Task<IList<string>> GetRolesAsync(ApplicationUser user);

    Task<ApplicationUser?> FindActiveUserByIdAsync(Guid userId);
}
