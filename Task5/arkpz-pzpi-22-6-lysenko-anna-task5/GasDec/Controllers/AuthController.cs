using GasDec.Models;
using GasDec.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly GasLeakDbContext _context;
    private readonly JwtService _jwtService;

    public AuthController(GasLeakDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    /// <summary>
    /// Виконує вхід користувача до системи.
    /// </summary>
    /// <param name="request">Об'єкт із обліковими даними користувача: електронна пошта та пароль.</param>
    /// <returns>JWT токен для авторизації або статус помилки.</returns>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = _context.Users.FirstOrDefault(u => u.email == request.Email
        && u.password == request.Password);

        if (user == null)
        {
            return Unauthorized("Невірні облікові дані.");
        }

        var token = _jwtService.GenerateToken(user.user_id.ToString(), user.role);

        return Ok(new { Token = token });
    }
}

public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}
