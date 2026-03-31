using Agendamiento.Data;
using Agendamiento.Data.Entities;
using Agendamiento.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace Agendamiento.Service;

public class ConfigService(AppDbContext db)
{
    static readonly string[] Dias = ["Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado"];

    // ── Servicios ─────────────────────────────────────────────
    public async Task<List<ServicioDto2>> GetServiciosAsync(int tenantId) =>
        await db.Servicios
            .Where(s => s.TenantId == tenantId)
            .OrderBy(s => s.Nombre)
            .Select(s => new ServicioDto2
            {
                Id = s.Id,
                Nombre = s.Nombre,
                DuracionMinutos = s.DuracionMinutos,
                Precio = s.Precio,
                Activo = s.Activo
            })
            .ToListAsync();

    public async Task<ServicioDto2> CrearServicioAsync(int tenantId, ServicioRequest req)
    {
        var s = new Servicio
        {
            TenantId = tenantId,
            Nombre = req.Nombre,
            DuracionMinutos = req.DuracionMinutos,
            Precio = req.Precio
        };
        db.Servicios.Add(s);
        await db.SaveChangesAsync();
        return new ServicioDto2
        {
            Id = s.Id,
            Nombre = s.Nombre,
            DuracionMinutos = s.DuracionMinutos,
            Precio = s.Precio,
            Activo = s.Activo
        };
    }

    public async Task<bool> EditarServicioAsync(int tenantId, int id, ServicioRequest req)
    {
        var s = await db.Servicios.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId);
        if (s is null) return false;
        s.Nombre = req.Nombre; s.DuracionMinutos = req.DuracionMinutos; s.Precio = req.Precio;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EliminarServicioAsync(int tenantId, int id)
    {
        var s = await db.Servicios.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId);
        if (s is null) return false;
        s.Activo = !s.Activo;
        await db.SaveChangesAsync();
        return true;
    }

    // ── Profesionales ─────────────────────────────────────────
    public async Task<List<ProfesionalDto2>> GetProfesionalesAsync(int tenantId) =>
        await db.Profesionales
            .Where(p => p.TenantId == tenantId)
            .OrderBy(p => p.Nombre)
            .Select(p => new ProfesionalDto2
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                FotoUrl = p.FotoUrl,
                Activo = p.Activo
            })
            .ToListAsync();

    public async Task<ProfesionalDto2> CrearProfesionalAsync(int tenantId, ProfesionalRequest req)
    {
        var p = new Profesional
        {
            TenantId = tenantId,
            Nombre = req.Nombre,
            Descripcion = req.Descripcion,
            FotoUrl = req.FotoUrl
        };
        db.Profesionales.Add(p);
        await db.SaveChangesAsync();
        return new ProfesionalDto2
        {
            Id = p.Id,
            Nombre = p.Nombre,
            Descripcion = p.Descripcion,
            FotoUrl = p.FotoUrl,
            Activo = p.Activo
        };
    }

    public async Task<bool> EditarProfesionalAsync(int tenantId, int id, ProfesionalRequest req)
    {
        var p = await db.Profesionales.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId);
        if (p is null) return false;
        p.Nombre = req.Nombre; p.Descripcion = req.Descripcion; p.FotoUrl = req.FotoUrl;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EliminarProfesionalAsync(int tenantId, int id)
    {
        var p = await db.Profesionales.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId);
        if (p is null) return false;
        p.Activo = !p.Activo;
        await db.SaveChangesAsync();
        return true;
    }

    // ── Disponibilidad ────────────────────────────────────────
    public async Task<List<DisponibilidadDto>> GetDisponibilidadAsync(int tenantId) =>
        await db.Disponibilidades
            .Include(d => d.Profesional)
            .Where(d => d.Profesional.TenantId == tenantId && d.Activo)
            .OrderBy(d => d.ProfesionalId).ThenBy(d => d.DiaSemana)
            .Select(d => new DisponibilidadDto
            {
                Id = d.Id,
                ProfesionalId = d.ProfesionalId,
                Profesional = d.Profesional.Nombre,
                DiaSemana = d.DiaSemana,
                DiaNombre = Dias[d.DiaSemana],
                HoraInicio = d.HoraInicio.ToString("HH:mm"),
                HoraFin = d.HoraFin.ToString("HH:mm")
            })
            .ToListAsync();

    public async Task<DisponibilidadDto?> CrearDisponibilidadAsync(int tenantId, DisponibilidadRequest req)
    {
        var prof = await db.Profesionales
            .FirstOrDefaultAsync(p => p.Id == req.ProfesionalId && p.TenantId == tenantId);
        if (prof is null) return null;

        var d = new Disponibilidad
        {
            ProfesionalId = req.ProfesionalId,
            DiaSemana = req.DiaSemana,
            HoraInicio = TimeOnly.Parse(req.HoraInicio),
            HoraFin = TimeOnly.Parse(req.HoraFin)
        };
        db.Disponibilidades.Add(d);
        await db.SaveChangesAsync();

        return new DisponibilidadDto
        {
            Id = d.Id,
            ProfesionalId = d.ProfesionalId,
            Profesional = prof.Nombre,
            DiaSemana = d.DiaSemana,
            DiaNombre = Dias[d.DiaSemana],
            HoraInicio = d.HoraInicio.ToString("HH:mm"),
            HoraFin = d.HoraFin.ToString("HH:mm")
        };
    }

    public async Task<bool> EliminarDisponibilidadAsync(int tenantId, int id)
    {
        var d = await db.Disponibilidades
            .Include(x => x.Profesional)
            .FirstOrDefaultAsync(x => x.Id == id && x.Profesional.TenantId == tenantId);
        if (d is null) return false;
        d.Activo = false;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<NegocioDto?> GetNegocioAsync(int tenantId)
    {
        return await db.Tenants
            .Where(t => t.Id == tenantId)
            .Select(t => new NegocioDto
            {
                Nombre = t.Nombre,
                Slug = t.Slug,
                Descripcion = t.Descripcion,
                Telefono = t.Telefono,
                Direccion = t.Direccion,
                Instagram = t.Instagram,
                LogoUrl = t.LogoUrl,
                FotoPortada = t.FotoPortada,
                ColorPrimario = t.ColorPrimario
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> EditarNegocioAsync(int tenantId, NegocioRequest req)
    {
        var tenant = await db.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId);
        if (tenant is null) return false;

        tenant.Nombre = req.Nombre;
        tenant.Descripcion = req.Descripcion;
        tenant.Telefono = req.Telefono;
        tenant.Direccion = req.Direccion;
        tenant.Instagram = req.Instagram;
        tenant.LogoUrl = req.LogoUrl;
        tenant.FotoPortada = req.FotoPortada;
        tenant.ColorPrimario = req.ColorPrimario;
        await db.SaveChangesAsync();
        return true;
    }
}