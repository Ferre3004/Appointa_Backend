using Agendamiento.Service;
using Agendamiento.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Agendamiento.Api.Controllers;

[ApiController]
[Route("api/admin/config")]
[Authorize]
public class ConfigController(ConfigService svc) : ControllerBase
{
    private int TenantId => int.Parse(User.FindFirstValue("TenantId")!);

    // Servicios
    [HttpGet("servicios")]
    public async Task<IActionResult> GetServicios()
        => Ok(await svc.GetServiciosAsync(TenantId));

    [HttpPost("servicios")]
    public async Task<IActionResult> CrearServicio(ServicioRequest req)
        => Ok(await svc.CrearServicioAsync(TenantId, req));

    [HttpPut("servicios/{id}")]
    public async Task<IActionResult> EditarServicio(int id, ServicioRequest req)
        => await svc.EditarServicioAsync(TenantId, id, req) ? NoContent() : NotFound();

    [HttpDelete("servicios/{id}")]
    public async Task<IActionResult> EliminarServicio(int id)
        => await svc.EliminarServicioAsync(TenantId, id) ? NoContent() : NotFound();

    // Profesionales
    [HttpGet("profesionales")]
    public async Task<IActionResult> GetProfesionales()
        => Ok(await svc.GetProfesionalesAsync(TenantId));

    [HttpPost("profesionales")]
    public async Task<IActionResult> CrearProfesional(ProfesionalRequest req)
        => Ok(await svc.CrearProfesionalAsync(TenantId, req));

    [HttpPut("profesionales/{id}")]
    public async Task<IActionResult> EditarProfesional(int id, ProfesionalRequest req)
        => await svc.EditarProfesionalAsync(TenantId, id, req) ? NoContent() : NotFound();

    [HttpDelete("profesionales/{id}")]
    public async Task<IActionResult> EliminarProfesional(int id)
        => await svc.EliminarProfesionalAsync(TenantId, id) ? NoContent() : NotFound();

    // Disponibilidad
    [HttpGet("disponibilidad")]
    public async Task<IActionResult> GetDisponibilidad()
        => Ok(await svc.GetDisponibilidadAsync(TenantId));

    [HttpPost("disponibilidad")]
    public async Task<IActionResult> CrearDisponibilidad(DisponibilidadRequest req)
    {
        var result = await svc.CrearDisponibilidadAsync(TenantId, req);
        return result is null ? BadRequest("Profesional inválido") : Ok(result);
    }

    [HttpDelete("disponibilidad/{id}")]
    public async Task<IActionResult> EliminarDisponibilidad(int id)
        => await svc.EliminarDisponibilidadAsync(TenantId, id) ? NoContent() : NotFound();

    [HttpGet("negocio")]
    public async Task<IActionResult> GetNegocio()
    {
        var result = await svc.GetNegocioAsync(TenantId);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPut("negocio")]
    public async Task<IActionResult> EditarNegocio(NegocioRequest req)
    {
        var ok = await svc.EditarNegocioAsync(TenantId, req);
        return ok ? NoContent() : NotFound();
    }
}