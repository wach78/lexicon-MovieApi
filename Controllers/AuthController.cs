using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Azure.Core;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using MovieApi.DTOs.Auth;
using MovieApi.Interfaces.Service;
using MovieApi.Models.Identity;
using MovieApi.Services;

namespace MovieApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private const string AccessTokenCookieName = "accessToken";
    private const string RefreshTokenCookieName = "refreshToken";

    private readonly IConfiguration _configuration;
    private readonly IAntiforgery _antiforgery;
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;
    private readonly IAuthCookieService _authCookieService;

    private static readonly ConcurrentDictionary<
        string,
        (Guid UserId, DateTime ExpiresAt)
    > RefreshTokens = new();

    public AuthController(
        IConfiguration configuration,
        IAntiforgery antiforgery,
        IAuthService authService,
        ITokenService tokenService,
        IAuthCookieService authCookieService)
    {
        _configuration = configuration;
        _antiforgery = antiforgery;
        _authService = authService;
        _tokenService = tokenService;
        _authCookieService = authCookieService;
    }

    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("LoginLimit")]
    public async Task<IActionResult> Login(
        [FromBody] LoginDto request)
    {
        ApplicationUser? user =
            await _authService.AuthenticateAsync(request);

        if (user is null)
        {
            return Unauthorized();
        }

        IList<string> roles =
            await _authService.GetRolesAsync(user);

        string accessToken = _tokenService.GenerateAccessToken(user, roles);

        string refreshToken = _tokenService.GenerateRefreshToken();

        RefreshTokens[refreshToken] = (
            user.Id,
            DateTime.UtcNow.AddDays(
                JwtConstants.RefreshTokenExpirationDays
            )
        );

        _authCookieService.SetAccessTokenCookie(Response, accessToken);

        _authCookieService.SetRefreshTokenCookie(Response, refreshToken);

        return NoContent();
    }

    [HttpPost("refresh")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Refresh()
    {
        string? currentRefreshToken =
            Request.Cookies[RefreshTokenCookieName];

        if (string.IsNullOrWhiteSpace(currentRefreshToken))
        {
            return Unauthorized();
        }

        if (!RefreshTokens.TryGetValue(
                currentRefreshToken,
                out var storedToken))
        {
            return Unauthorized();
        }

        if (storedToken.ExpiresAt <= DateTime.UtcNow)
        {
            RefreshTokens.TryRemove(
                currentRefreshToken,
                out _
            );

            _authCookieService.DeleteAuthCookies(Response);

            return Unauthorized();
        }

        ApplicationUser? user =
            await _authService.FindActiveUserByIdAsync(
                storedToken.UserId
            );

        if (user is null)
        {
            RefreshTokens.TryRemove(
                currentRefreshToken,
                out _
            );

            _authCookieService.DeleteAuthCookies(Response);

            return Unauthorized();
        }

        IList<string> roles =
            await _authService.GetRolesAsync(user);

        // Rotate refresh token
        RefreshTokens.TryRemove(
            currentRefreshToken,
            out _
        );

        string newAccessToken = _tokenService.GenerateAccessToken(user, roles);

        string newRefreshToken = _tokenService.GenerateRefreshToken();

        RefreshTokens[newRefreshToken] = (
            user.Id,
            DateTime.UtcNow.AddDays(
                JwtConstants.RefreshTokenExpirationDays
            )
        );

        _authCookieService.SetAccessTokenCookie(Response, newAccessToken);

        _authCookieService.SetRefreshTokenCookie(Response, newRefreshToken);

        return NoContent();
    }

    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        string? refreshToken =
            Request.Cookies[RefreshTokenCookieName];

        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            RefreshTokens.TryRemove(
                refreshToken,
                out _
            );
        }

        _authCookieService.DeleteAuthCookies(Response);

        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            UserId = User.FindFirstValue(
                ClaimTypes.NameIdentifier
            ),
            Email = User.FindFirstValue(
                ClaimTypes.Email
            ),
            Roles = User.FindAll(
                    ClaimTypes.Role
                )
                .Select(claim => claim.Value)
                .ToArray()
        });
    }

    [HttpGet("csrf")]
    public IActionResult GetCsrfToken()
    {
        AntiforgeryTokenSet tokens =
            _antiforgery.GetAndStoreTokens(
                HttpContext
            );

        return Ok(new
        {
            CsrfToken = tokens.RequestToken
        });
    }
}
