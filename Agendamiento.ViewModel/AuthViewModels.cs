namespace Agendamiento.ViewModel;

public class LoginRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class LoginResponse
{
    public string Token { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string Rol { get; set; } = null!;
    public string? SuscripcionEstado { get; set; }
}
public class RegistroRequest
{
    public string NombreNegocio { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string Telefono { get; set; } = null!;
    public string Rubro { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string? Direccion { get; set; }
    public string? Instagram { get; set; }
    public string? LogoUrl { get; set; }
    public string? FotoPortada { get; set; }
    public string? ColorPrimario { get; set; }
    public string AdminNombre { get; set; } = null!;
    public string AdminEmail { get; set; } = null!;
    public string AdminPassword { get; set; } = null!;
}

public class RegistroResponse
{
    public string Token { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string Rol { get; set; } = null!;
    public string Slug { get; set; } = null!;
}