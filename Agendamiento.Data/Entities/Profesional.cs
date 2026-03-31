namespace Agendamiento.Data.Entities;

public class Profesional
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string? FotoUrl { get; set; }
    public bool Activo { get; set; } = true;

    public Tenant Tenant { get; set; } = null!;
    public ICollection<Disponibilidad> Disponibilidades { get; set; } = [];
    public ICollection<Reserva> Reservas { get; set; } = [];
}