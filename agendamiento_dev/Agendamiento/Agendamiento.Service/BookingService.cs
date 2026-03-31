using Agendamiento.Data;
using Agendamiento.ViewModel;
using Microsoft.EntityFrameworkCore;
using Agendamiento.Data.Entities;
using Hangfire;

namespace Agendamiento.Service;

public class BookingService(AppDbContext db, IBackgroundJobClient jobClient)
{
    public async Task<TenantPublicoDto?> GetTenantBySlugAsync(string slug)
    {
        return await db.Tenants
            .Where(t => t.Slug == slug && t.Activo)
            .Select(t => new TenantPublicoDto
            {
                Nombre = t.Nombre,
                LogoUrl = t.LogoUrl,
                FotoPortada = t.FotoPortada,
                ColorPrimario = t.ColorPrimario,
                Telefono = t.Telefono,
                Descripcion = t.Descripcion,
                Direccion = t.Direccion,
                Instagram = t.Instagram
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<ServicioDto>> GetServiciosAsync(int tenantId)
    {
        return await db.Servicios
            .Where(s => s.TenantId == tenantId && s.Activo)
            .Select(s => new ServicioDto
            {
                Id = s.Id,
                Nombre = s.Nombre,
                DuracionMinutos = s.DuracionMinutos,
                Precio = s.Precio
            })
            .ToListAsync();
    }

    public async Task<List<ProfesionalDto>> GetProfesionalesAsync(int tenantId)
    {
        return await db.Profesionales
            .Where(p => p.TenantId == tenantId && p.Activo)
            .Select(p => new ProfesionalDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                FotoUrl = p.FotoUrl
            })
            .ToListAsync();
    }

    public async Task<List<SlotDto>> GetSlotsDisponiblesAsync(
        int tenantId, int profesionalId, int servicioId, DateOnly fecha)
    {
        // Validar que profesional y servicio pertenezcan al tenant
        var servicio = await db.Servicios
            .FirstOrDefaultAsync(s => s.Id == servicioId && s.TenantId == tenantId);
        if (servicio is null) return [];

        var diaSemana = (short)fecha.DayOfWeek;

        var disponibilidad = await db.Disponibilidades
            .Where(d => d.ProfesionalId == profesionalId && d.DiaSemana == diaSemana && d.Activo)
            .FirstOrDefaultAsync();
        if (disponibilidad is null) return [];

        // Reservas ya ocupadas ese día
        var fechaInicio = fecha.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var fechaFin = fechaInicio.AddDays(1);

        var reservasOcupadas = await db.Reservas
            .Where(r => r.ProfesionalId == profesionalId
                     && r.FechaHora >= fechaInicio
                     && r.FechaHora < fechaFin
                     && r.Estado != "Cancelada")
            .Select(r => new { r.FechaHora, r.DuracionMinutos })
            .ToListAsync();

        // Generar slots libres
        var slots = new List<SlotDto>();
        var duracion = TimeSpan.FromMinutes(servicio.DuracionMinutos);
        var cursor = fecha.ToDateTime(disponibilidad.HoraInicio, DateTimeKind.Utc);
        var tope = fecha.ToDateTime(disponibilidad.HoraFin, DateTimeKind.Utc);

        while (cursor + duracion <= tope)
        {
            var ocupado = reservasOcupadas.Any(r =>
                cursor < r.FechaHora.AddMinutes(r.DuracionMinutos) &&
                cursor.AddMinutes(servicio.DuracionMinutos) > r.FechaHora);

            if (!ocupado)
            {
                slots.Add(new SlotDto
                {
                    FechaHora = cursor,
                    Etiqueta = cursor.ToString("HH:mm")
                });
            }

            cursor += duracion;
        }

        return slots;
    }

    public async Task<ReservaConfirmadaDto?> CrearReservaAsync(int tenantId, CrearReservaRequest req)
    {
        var servicio = await db.Servicios
            .FirstOrDefaultAsync(s => s.Id == req.ServicioId && s.TenantId == tenantId);
        var profesional = await db.Profesionales
            .FirstOrDefaultAsync(p => p.Id == req.ProfesionalId && p.TenantId == tenantId);

        if (servicio is null || profesional is null) return null;

        var reserva = new Data.Entities.Reserva
        {
            TenantId = tenantId,
            ProfesionalId = req.ProfesionalId,
            ServicioId = req.ServicioId,
            ClienteNombre = req.ClienteNombre,
            ClienteTelefono = req.ClienteTelefono,
            ClienteEmail = req.ClienteEmail,
            FechaHora = req.FechaHora.ToUniversalTime(),
            DuracionMinutos = servicio.DuracionMinutos,
            Estado = "Pendiente",
            RecordatorioEstado = "Pendiente"
        };

        db.Reservas.Add(reserva);
        await db.SaveChangesAsync();

        // Programar recordatorio 24hs antes del turno
        var cuandoEnviar = reserva.FechaHora.AddHours(-24);
        if (cuandoEnviar > DateTime.UtcNow)
        {
            jobClient.Schedule<RecordatorioJob>(
                job => job.EnviarRecordatorio(reserva.Id),
                cuandoEnviar
            );
        }

        return new ReservaConfirmadaDto
        {
            Id = reserva.Id,
            Servicio = servicio.Nombre,
            Profesional = profesional.Nombre,
            FechaHora = reserva.FechaHora,
            ClienteNombre = reserva.ClienteNombre
        };
    }
}