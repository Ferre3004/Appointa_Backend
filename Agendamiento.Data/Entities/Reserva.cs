namespace Agendamiento.Data.Entities;

public class Reserva
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int ProfesionalId { get; set; }
    public int ServicioId { get; set; }
    public string ClienteNombre { get; set; } = null!;
    public string ClienteTelefono { get; set; } = null!;
    public string? ClienteEmail { get; set; }
    public DateTime FechaHora { get; set; }
    public int DuracionMinutos { get; set; }
    public string Estado { get; set; } = "Pendiente";
    public string RecordatorioEstado { get; set; } = "Pendiente";
    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

    public Tenant Tenant { get; set; } = null!;
    public Profesional Profesional { get; set; } = null!;
    public Servicio Servicio { get; set; } = null!;
}