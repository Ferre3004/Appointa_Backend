using Agendamiento.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agendamiento.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var tenants = await db.Tenants
            .Where(t => t.Activo)
            .Select(t => new { t.Id, t.Nombre, t.Slug })
            .ToListAsync();

        return Ok(tenants);
    }
}