namespace Agendamiento.Data.Entities;

public class Usuario
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string Nombre { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Rol { get; set; } = "Admin";
    public bool Activo { get; set; } = true;

    public Tenant Tenant { get; set; } = null!;
}