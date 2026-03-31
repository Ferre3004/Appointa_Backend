using Agendamiento.Service;
using Agendamiento.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Agendamiento.Api.Controllers;

[ApiController]
[Route("api/admin/reservas")]
[Authorize]
public class ReservasAdminController(ReservaAdminService svc) : ControllerBase
{
    private int TenantId => int.Parse(User.FindFirstValue("TenantId")!);

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] DateOnly? fecha)
        => Ok(await svc.GetReservasAsync(TenantId, fecha));

    [HttpPatch("{id}/estado")]
    public async Task<IActionResult> CambiarEstado(int id, CambiarEstadoRequest req)
    {
        var ok = await svc.CambiarEstadoAsync(TenantId, id, req.Estado);
        return ok ? NoContent() : NotFound();
    }
}