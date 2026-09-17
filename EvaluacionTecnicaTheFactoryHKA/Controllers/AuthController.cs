using EvaluacionTecnicaTheFactoryHKA.Aplication.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EvaluacionTecnicaTheFactoryHKA.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// Genera un token JWT (Login simluado para prueba técnica).
    /// </summary>
    /// <remarks>
    /// Usuario: admin | Contraseña: password123
    /// </remarks>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // Dummy check para efectos de la prueba
        if (request.Username == "admin" && request.Password == "password123")
        {
            var token = _authService.GenerateToken(request.Username);
            return Ok(new { Token = token });
        }

        return Unauthorized(new { error = "Credenciales incorrectas. Use admin / password123" });
    }
}
