using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EvaluacionTecnicaTheFactoryHKA.Aplication.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EvaluacionTecnicaTheFactoryHKA.Authentication.Services;

public class JwtAuthService : IAuthService
{
    private readonly IConfiguration _configuration;

    public JwtAuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(string username)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "super-secret-key-for-the-factory-hka-eval-2026-very-long";
        var key = Encoding.ASCII.GetBytes(secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "Admin") // Rol hardcodeado para la prueba técnica
            }),
            Expires = DateTime.UtcNow.AddHours(2), // Expira en 2 horas
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
