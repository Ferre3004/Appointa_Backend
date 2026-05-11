using System.ComponentModel.DataAnnotations;

namespace Agendamiento.ViewModel;

public class ServicioRequest
{
    [Required(ErrorMessage = "El nombre del servicio es requerido")]
    [MaxLength(200, ErrorMessage = "El nombre no puede superar 200 caracteres")]
    public string Nombre { get; set; } = null!;

    [Range(5, 480, ErrorMessage = "La duración debe estar entre 5 y 480 minutos")]
    public int DuracionMinutos { get; set; }

    [Range(0, 999999.99, ErrorMessage = "El precio debe ser mayor o igual a 0")]
    public decimal? Precio { get; set; }
}

public class ServicioDto2
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public int DuracionMinutos { get; set; }
    public decimal? Precio { get; set; }
    public bool Activo { get; set; }
}

public class ProfesionalRequest
{
    [Required(ErrorMessage = "El nombre del profesional es requerido")]
    [MaxLength(200, ErrorMessage = "El nombre no puede superar 200 caracteres")]
    public string Nombre { get; set; } = null!;

    [MaxLength(500, ErrorMessage = "La descripción no puede superar 500 caracteres")]
    public string? Descripcion { get; set; }

    [Url(ErrorMessage = "La foto debe ser una URL válida")]
    public string? FotoUrl { get; set; }
}

public class ProfesionalDto2
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string? FotoUrl { get; set; }
    public bool Activo { get; set; }
}

public class DisponibilidadRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "ProfesionalId inválido")]
    public int ProfesionalId { get; set; }

    [Range(0, 6, ErrorMessage = "DiaSemana debe ser entre 0 (Domingo) y 6 (Sábado)")]
    public short DiaSemana { get; set; }

    [Required(ErrorMessage = "La hora de inicio es requerida")]
    [RegularExpression(@"^([01]\d|2[0-3]):([0-5]\d)$", ErrorMessage = "HoraInicio debe tener formato HH:mm")]
    public string HoraInicio { get; set; } = null!;

    [Required(ErrorMessage = "La hora de fin es requerida")]
    [RegularExpression(@"^([01]\d|2[0-3]):([0-5]\d)$", ErrorMessage = "HoraFin debe tener formato HH:mm")]
    public string HoraFin { get; set; } = null!;
}

public class DisponibilidadDto
{
    public int Id { get; set; }
    public int ProfesionalId { get; set; }
    public string Profesional { get; set; } = null!;
    public short DiaSemana { get; set; }
    public string DiaNombre { get; set; } = null!;
    public string HoraInicio { get; set; } = null!;
    public string HoraFin { get; set; } = null!;
}

public class NegocioRequest
{
    [Required(ErrorMessage = "El nombre del negocio es requerido")]
    [MaxLength(200, ErrorMessage = "El nombre no puede superar 200 caracteres")]
    public string Nombre { get; set; } = null!;

    [MaxLength(500, ErrorMessage = "La descripción no puede superar 500 caracteres")]
    public string? Descripcion { get; set; }

    [Phone(ErrorMessage = "Formato de teléfono inválido")]
    [MaxLength(30, ErrorMessage = "El teléfono no puede superar 30 caracteres")]
    public string? Telefono { get; set; }

    [MaxLength(300, ErrorMessage = "La dirección no puede superar 300 caracteres")]
    public string? Direccion { get; set; }

    [MaxLength(100, ErrorMessage = "El Instagram no puede superar 100 caracteres")]
    public string? Instagram { get; set; }

    [Url(ErrorMessage = "El logo debe ser una URL válida")]
    public string? LogoUrl { get; set; }

    [Url(ErrorMessage = "La foto de portada debe ser una URL válida")]
    public string? FotoPortada { get; set; }

    [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "El color primario debe ser un hex válido (#RRGGBB)")]
    public string? ColorPrimario { get; set; }
}

public class NegocioDto : NegocioRequest
{
    public string Slug { get; set; } = null!;
}
