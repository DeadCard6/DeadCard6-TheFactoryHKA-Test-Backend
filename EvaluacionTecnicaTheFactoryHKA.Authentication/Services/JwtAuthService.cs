using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EvaluacionTecnicaTheFactoryHKA.Aplication.Interfaces;
using EvaluacionTecnicaTheFactoryHKA.Domain.Entities;
using EvaluacionTecnicaTheFactoryHKA.Domain.Exceptions;
using EvaluacionTecnicaTheFactoryHKA.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EvaluacionTecnicaTheFactoryHKA.Authentication.Services;

public class JwtAuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public JwtAuthService(IConfiguration configuration, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _configuration = configuration;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task RegisterAsync(string username, string password)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(username);
        if (existingUser != null)
        {
            throw new DomainException("Username already exists.");
        }

        var user = new User
        {
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<string> LoginAsync(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            throw new DomainException("Invalid credentials.");
        }

        return GenerateToken(user.Username);
    }

    private string GenerateToken(string username)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "super-secret-key-for-the-factory-hka-eval-2026-very-long";
        var key = Encoding.ASCII.GetBytes(secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "Admin")
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}

