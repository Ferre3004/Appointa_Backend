using System.ComponentModel.DataAnnotations;

namespace Agendamiento.ViewModel;

public class LoginRequest
{
    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "Formato de email inválido")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "La contraseña es requerida")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
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
    [Required(ErrorMessage = "El nombre del negocio es requerido")]
    [MaxLength(200, ErrorMessage = "El nombre no puede superar 200 caracteres")]
    public string NombreNegocio { get; set; } = null!;

    [Required(ErrorMessage = "El slug es requerido")]
    [MaxLength(100, ErrorMessage = "El slug no puede superar 100 caracteres")]
    [RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "El slug solo puede contener letras minúsculas, números y guiones")]
    public string Slug { get; set; } = null!;

    [Required(ErrorMessage = "El teléfono es requerido")]
    [Phone(ErrorMessage = "Formato de teléfono inválido")]
    [MaxLength(30, ErrorMessage = "El teléfono no puede superar 30 caracteres")]
    public string Telefono { get; set; } = null!;

    [Required(ErrorMessage = "El rubro es requerido")]
    [MaxLength(100, ErrorMessage = "El rubro no puede superar 100 caracteres")]
    public string Rubro { get; set; } = null!;

    [MaxLength(500, ErrorMessage = "La descripción no puede superar 500 caracteres")]
    public string? Descripcion { get; set; }

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

    [Required(ErrorMessage = "El nombre del administrador es requerido")]
    [MaxLength(200, ErrorMessage = "El nombre no puede superar 200 caracteres")]
    public string AdminNombre { get; set; } = null!;

    [Required(ErrorMessage = "El email del administrador es requerido")]
    [EmailAddress(ErrorMessage = "Formato de email inválido")]
    public string AdminEmail { get; set; } = null!;

    [Required(ErrorMessage = "La contraseña es requerida")]
    [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
    public string AdminPassword { get; set; } = null!;
}

public class RegistroResponse
{
    public string Token { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string Rol { get; set; } = null!;
    public string Slug { get; set; } = null!;
}
