namespace Agendamiento.ViewModel;

public class ServicioRequest
{
    public string Nombre { get; set; } = null!;
    public int DuracionMinutos { get; set; }
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
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
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
    public int ProfesionalId { get; set; }
    public short DiaSemana { get; set; }
    public string HoraInicio { get; set; } = null!;
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
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public string? Instagram { get; set; }
    public string? LogoUrl { get; set; }
    public string? FotoPortada { get; set; }
    public string? ColorPrimario { get; set; }
}

public class NegocioDto : NegocioRequest
{
    public string Slug { get; set; } = null!;
}