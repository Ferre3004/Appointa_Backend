namespace Agendamiento.Data.Entities;

public class Servicio
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string Nombre { get; set; } = null!;
    public int DuracionMinutos { get; set; }
    public decimal? Precio { get; set; }
    public bool Activo { get; set; } = true;

    public Tenant Tenant { get; set; } = null!;
    public ICollection<Reserva> Reservas { get; set; } = [];
}