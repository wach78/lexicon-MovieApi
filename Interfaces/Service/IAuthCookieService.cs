namespace MovieApi.Interfaces.Service;

public interface IAuthCookieService
{
    void SetAccessTokenCookie(HttpResponse response, string accessToken);

    void SetRefreshTokenCookie(HttpResponse response, string refreshToken);

    void DeleteAuthCookies(HttpResponse response);
}
