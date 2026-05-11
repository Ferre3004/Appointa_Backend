using System.ComponentModel.DataAnnotations;

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
    public string Etiqueta { get; set; } = null!;
}

public class CrearReservaRequest
{
    [Required(ErrorMessage = "El profesional es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "ProfesionalId inválido")]
    public int ProfesionalId { get; set; }

    [Required(ErrorMessage = "El servicio es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "ServicioId inválido")]
    public int ServicioId { get; set; }

    [Required(ErrorMessage = "La fecha y hora son requeridas")]
    public DateTime FechaHora { get; set; }

    [Required(ErrorMessage = "El nombre del cliente es requerido")]
    [MaxLength(200, ErrorMessage = "El nombre no puede superar 200 caracteres")]
    public string ClienteNombre { get; set; } = null!;

    [Required(ErrorMessage = "El teléfono del cliente es requerido")]
    [Phone(ErrorMessage = "Formato de teléfono inválido")]
    [MaxLength(30, ErrorMessage = "El teléfono no puede superar 30 caracteres")]
    public string ClienteTelefono { get; set; } = null!;

    [EmailAddress(ErrorMessage = "Formato de email inválido")]
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
