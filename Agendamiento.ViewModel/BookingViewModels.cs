namespace Agendamiento.ViewModel;

public class TenantPublicoDto
{
    public string Nombre { get; set; } = null!;
    public string? LogoUrl { get; set; }
    public string? FotoPortada { get; set; }
    public string? ColorPrimario { get; set; }
    public string? Telefono { get; set; }
    public string? Descripcion { get; set; }
    public string? Direccion { get; set; }
    public string? Instagram { get; set; }
}

public class ServicioDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public int DuracionMinutos { get; set; }
    public decimal? Precio { get; set; }
}

public class ProfesionalDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string? FotoUrl { get; set; }
}

public class SlotDto
{
    public DateTime FechaHora { get; set; }
    public string Etiqueta { get; set; } = null!; // "09:00"
}

public class CrearReservaRequest
{
    public int ProfesionalId { get; set; }
    public int ServicioId { get; set; }
    public DateTime FechaHora { get; set; }
    public string ClienteNombre { get; set; } = null!;
    public string ClienteTelefono { get; set; } = null!;
    public string? ClienteEmail { get; set; }
}

public class ReservaConfirmadaDto
{
    public int Id { get; set; }
    public string Servicio { get; set; } = null!;
    public string Profesional { get; set; } = null!;
    public DateTime FechaHora { get; set; }
    public string ClienteNombre { get; set; } = null!;
}