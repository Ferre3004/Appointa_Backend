using Agendamiento.Service;
using Agendamiento.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Agendamiento.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthService auth, LemonSqueezyService lsService) : ControllerBase
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

    [HttpPost("checkout")]
    [Authorize]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequest req)
    {
        var tenantIdClaim = User.FindFirst("TenantId")?.Value;
        if (tenantIdClaim is null) return Unauthorized();

        var tenantId = int.Parse(tenantIdClaim);
        var email = User.FindFirst(ClaimTypes.Email)?.Value ?? "";
        var nombre = User.FindFirst(ClaimTypes.Name)?.Value ?? "";

        var url = await lsService.CrearCheckoutAsync(tenantId, req.Plan, email, nombre);
        if (url is null) return BadRequest("No se pudo generar el checkout");

        return Ok(new { checkoutUrl = url });
    }
}