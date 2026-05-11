using System.ComponentModel.DataAnnotations;

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
    [Required(ErrorMessage = "El estado es requerido")]
    [RegularExpression("^(Confirmada|Cancelada|Completada|Pendiente)$",
        ErrorMessage = "Estado debe ser: Confirmada, Cancelada, Completada o Pendiente")]
    public string Estado { get; set; } = null!;
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = [];
    public int Total { get; set; }
    public int Pagina { get; set; }
    public int TamañoPagina { get; set; }
    public int TotalPaginas => (int)Math.Ceiling((double)Total / TamañoPagina);
}
