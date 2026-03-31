using Agendamiento.Data;
using Agendamiento.Service;
using Agendamiento.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agendamiento.Api.Controllers;

[ApiController]
[Route("api/booking/{slug}")]
public class BookingController(BookingService booking, AppDbContext db) : ControllerBase
{
    private async Task<int?> GetTenantId(string slug)
    {
        return await db.Tenants
            .Where(t => t.Slug == slug && t.Activo)
            .Select(t => (int?)t.Id)
            .FirstOrDefaultAsync();
    }

    [HttpGet]
    public async Task<IActionResult> GetTenant(string slug)
    {
        var tenant = await booking.GetTenantBySlugAsync(slug);
        if (tenant is null) return NotFound();
        return Ok(tenant);
    }

    [HttpGet("servicios")]
    public async Task<IActionResult> GetServicios(string slug)
    {
        var tenantId = await GetTenantId(slug);
        if (tenantId is null) return NotFound();
        return Ok(await booking.GetServiciosAsync(tenantId.Value));
    }

    [HttpGet("profesionales")]
    public async Task<IActionResult> GetProfesionales(string slug)
    {
        var tenantId = await GetTenantId(slug);
        if (tenantId is null) return NotFound();
        return Ok(await booking.GetProfesionalesAsync(tenantId.Value));
    }

    [HttpGet("slots")]
    public async Task<IActionResult> GetSlots(
        string slug,
        [FromQuery] int profesionalId,
        [FromQuery] int servicioId,
        [FromQuery] DateOnly fecha)
    {
        var tenantId = await GetTenantId(slug);
        if (tenantId is null) return NotFound();
        var slots = await booking.GetSlotsDisponiblesAsync(tenantId.Value, profesionalId, servicioId, fecha);
        return Ok(slots);
    }

    [HttpPost("reservar")]
    public async Task<IActionResult> Reservar(string slug, CrearReservaRequest req)
    {
        var tenantId = await GetTenantId(slug);
        if (tenantId is null) return NotFound();
        var result = await booking.CrearReservaAsync(tenantId.Value, req);
        if (result is null) return BadRequest("Profesional o servicio inválido");
        return Ok(result);
    }
}