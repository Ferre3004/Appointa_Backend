namespace Agendamiento.ViewModel;

public class ReservaAdminDto
{
    public int Id { get; set; }
    public string ClienteNombre { get; set; } = null!;
    public string ClienteTelefono { get; set; } = null!;
    public string? ClienteEmail { get; set; }
    public string Servicio { get; set; } = null!;
    public string Profesional { get; set; } = null!;
    public DateTime FechaHora { get; set; }
    public int DuracionMinutos { get; set; }
    public string Estado { get; set; } = null!;
    public DateTime CreadoEn { get; set; }
}

public class CambiarEstadoRequest
{
    public string Estado { get; set; } = null!; // Confirmada | Cancelada | Completada
}