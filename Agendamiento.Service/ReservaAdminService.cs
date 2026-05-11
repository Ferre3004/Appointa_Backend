using Agendamiento.Data;
using Agendamiento.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace Agendamiento.Service;

public class ReservaAdminService(AppDbContext db)
{
    public async Task<PagedResult<ReservaAdminDto>> GetReservasAsync(
        int tenantId,
        DateOnly? fecha = null,
        int pagina = 1,
        int tamañoPagina = 20)
    {
        tamañoPagina = Math.Clamp(tamañoPagina, 1, 100);
        pagina = Math.Max(1, pagina);

        var query = db.Reservas
            .Include(r => r.Servicio)
            .Include(r => r.Profesional)
            .Where(r => r.TenantId == tenantId);

        if (fecha.HasValue)
        {
            var desde = fecha.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var hasta = desde.AddDays(1);
            query = query.Where(r => r.FechaHora >= desde && r.FechaHora < hasta);
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(r => r.FechaHora)
            .Skip((pagina - 1) * tamañoPagina)
            .Take(tamañoPagina)
            .Select(r => new ReservaAdminDto
            {
                Id = r.Id,
                ClienteNombre = r.ClienteNombre,
                ClienteTelefono = r.ClienteTelefono,
                ClienteEmail = r.ClienteEmail,
                Servicio = r.Servicio.Nombre,
                Profesional = r.Profesional.Nombre,
                FechaHora = r.FechaHora,
                DuracionMinutos = r.DuracionMinutos,
                Estado = r.Estado,
                CreadoEn = r.CreadoEn
            })
            .ToListAsync();

        return new PagedResult<ReservaAdminDto>
        {
            Items = items,
            Total = total,
            Pagina = pagina,
            TamañoPagina = tamañoPagina
        };
    }

    public async Task<bool> CambiarEstadoAsync(int tenantId, int reservaId, string estado)
    {
        var reserva = await db.Reservas
            .FirstOrDefaultAsync(r => r.Id == reservaId && r.TenantId == tenantId);

        if (reserva is null) return false;

        reserva.Estado = estado;
        await db.SaveChangesAsync();
        return true;
    }
}
