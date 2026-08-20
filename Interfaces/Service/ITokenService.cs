using MovieApi.Models.Identity;

namespace MovieApi.Interfaces.Service;

public interface ITokenService
{
    string GenerateAccessToken(
        ApplicationUser user,
        IList<string> roles
    );

    string GenerateRefreshToken();
}
