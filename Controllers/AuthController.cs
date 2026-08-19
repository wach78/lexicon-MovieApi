using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MovieApi.DTOs.Auth;

namespace MovieApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private const string AccessTokenCookieName = "accessToken";
    private const string RefreshTokenCookieName = "refreshToken";

    private readonly IConfiguration _configuration;

    private static readonly ConcurrentDictionary<
        string,
        (string Username, DateTime ExpiresAt)
    > RefreshTokens = new();

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto request)
    {
        // Hardcoded user for JWT exercise
        if (request.Username != "admin" || request.Password != "1234")
        {
            return Unauthorized();
        }

        string accessToken = GenerateAccessToken(request.Username);
        string refreshToken = GenerateRefreshToken();

        RefreshTokens[refreshToken] = (
            request.Username,
            DateTime.UtcNow.AddDays(
                JwtConstants.RefreshTokenExpirationDays
            )
        );

        SetAccessTokenCookie(accessToken);
        SetRefreshTokenCookie(refreshToken);

        return NoContent();
    }

    [HttpPost("refresh")]
    public IActionResult Refresh()
    {
        string? currentRefreshToken = Request.Cookies[RefreshTokenCookieName];

        if (string.IsNullOrWhiteSpace(currentRefreshToken))
        {
            return Unauthorized();
        }

        if (!RefreshTokens.TryGetValue(currentRefreshToken, out var storedToken))
        {
            return Unauthorized();
        }

        if (storedToken.ExpiresAt <= DateTime.UtcNow)
        {
            RefreshTokens.TryRemove(currentRefreshToken, out _);

            DeleteAuthCookies();

            return Unauthorized();
        }

        // Remove old refresh token
        RefreshTokens.TryRemove(currentRefreshToken, out _);

        // Create new tokens
        string newAccessToken = GenerateAccessToken(storedToken.Username);

        string newRefreshToken = GenerateRefreshToken();

        RefreshTokens[newRefreshToken] = (
            storedToken.Username,
            DateTime.UtcNow.AddDays(
                JwtConstants.RefreshTokenExpirationDays
            )
        );

        SetAccessTokenCookie(newAccessToken);
        SetRefreshTokenCookie(newRefreshToken);

        return NoContent();
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        string? refreshToken = Request.Cookies[RefreshTokenCookieName];

        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            RefreshTokens.TryRemove(refreshToken, out _);
        }

        DeleteAuthCookies();

        return NoContent();
    }

    private string GenerateAccessToken(string username)
    {
        Claim[] claims =
        [
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, "Admin")
        ];

        string secretKey = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException(
                "JWT key is missing."
            );

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(secretKey));

        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);

        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(
                JwtConstants.AccessTokenExpirationMinutes
            ),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = credentials
        };

        JwtSecurityTokenHandler tokenHandler = new();

        SecurityToken token =
            tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        byte[] randomBytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(randomBytes);
    }

    private void SetAccessTokenCookie(string accessToken)
    {
        Response.Cookies.Append(
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

    private void SetRefreshTokenCookie(string refreshToken)
    {
        Response.Cookies.Append(
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

    private void DeleteAuthCookies()
    {
        Response.Cookies.Delete(
            AccessTokenCookieName,
            new CookieOptions
            {
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            }
        );

        Response.Cookies.Delete(
            RefreshTokenCookieName,
            new CookieOptions
            {
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/api/auth"
            }
        );
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            Username = User.Identity?.Name
        });
    }
}
