using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MovieApi.Interfaces.Service;
using MovieApi.Models.Identity;

namespace MovieApi.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateAccessToken(
        ApplicationUser user,
        IList<string> roles)
    {
        string email = user.Email
            ?? throw new InvalidOperationException(
                "User email is missing."
            );

        List<Claim> claims =
        [
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()
            ),
            new Claim(
                ClaimTypes.Name,
                email
            ),
            new Claim(
                ClaimTypes.Email,
                email
            )
        ];

        foreach (string role in roles)
        {
            claims.Add(
                new Claim(
                    ClaimTypes.Role,
                    role
                )
            );
        }

        string secretKey =
            _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException(
                "JWT key is missing."
            );

        SymmetricSecurityKey key =
            new(
                Encoding.UTF8.GetBytes(
                    secretKey
                )
            );

        SigningCredentials credentials =
            new(
                key,
                SecurityAlgorithms.HmacSha256
            );

        SecurityTokenDescriptor tokenDescriptor =
            new()
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
            tokenHandler.CreateToken(
                tokenDescriptor
            );

        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        byte[] randomBytes =
            RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(
            randomBytes
        );
    }
}
