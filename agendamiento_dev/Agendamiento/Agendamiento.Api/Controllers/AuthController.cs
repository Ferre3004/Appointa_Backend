using Agendamiento.Service;
using Agendamiento.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Agendamiento.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthService auth) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        var result = await auth.LoginAsync(req.Email, req.Password);
        if (result is null) return Unauthorized("Credenciales incorrectas");
        return Ok(result);
    }

    [HttpPost("registro")]
    public async Task<IActionResult> Registro(RegistroRequest req)
    {
        var (result, error) = await auth.RegistrarAsync(req);
        if (error is not null) return BadRequest(new { error });
        return Ok(result);
    }
}