using MovieApi.Interfaces.Service;

namespace MovieApi.Services;

public class AuthCookieService : IAuthCookieService
{
    private const string AccessTokenCookieName = "accessToken";
    private const string RefreshTokenCookieName = "refreshToken";

    public void SetAccessTokenCookie(
        HttpResponse response,
        string accessToken)
    {
        response.Cookies.Append(
            AccessTokenCookieName,
            accessToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(
                    JwtConstants.AccessTokenExpirationMinutes
                ),
                Path = "/"
            }
        );
    }

    public void SetRefreshTokenCookie(HttpResponse response, string refreshToken)
    {
        response.Cookies.Append(
            RefreshTokenCookieName,
            refreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(
                    JwtConstants.RefreshTokenExpirationDays
                ),
                Path = "/api/auth"
            }
        );
    }

    public void DeleteAuthCookies(HttpResponse response)
    {
        response.Cookies.Delete(
            AccessTokenCookieName,
            new CookieOptions
            {
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/"
            }
        );

        response.Cookies.Delete(
            RefreshTokenCookieName,
            new CookieOptions
            {
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/api/auth"
            }
        );
    }
}
