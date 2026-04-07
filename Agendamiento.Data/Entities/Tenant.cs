namespace Agendamiento.Data.Entities;

public class Tenant
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? LogoUrl { get; set; }
    public string? ColorPrimario { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public bool Activo { get; set; } = true;
    public string? Descripcion { get; set; }
    public string? Direccion { get; set; }
    public string? Instagram { get; set; }
    public string? FotoPortada { get; set; }
    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

    // ── Suscripción LemonSqueezy ──────────────────────────
    public string? PlanNombre { get; set; }
    public string? LemonSqueezySubscriptionId { get; set; }
    public string? LemonSqueezyCustomerId { get; set; }
    public string? SuscripcionEstado { get; set; }
    public DateTime? SuscripcionVence { get; set; }

    public ICollection<Usuario> Usuarios { get; set; } = [];
    public ICollection<Profesional> Profesionales { get; set; } = [];
    public ICollection<Servicio> Servicios { get; set; } = [];
    public ICollection<Reserva> Reservas { get; set; } = [];
}
