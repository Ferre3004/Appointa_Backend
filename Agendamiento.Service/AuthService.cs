using Agendamiento.Data;
using Agendamiento.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Agendamiento.Service;

public class AuthService(AppDbContext db, IConfiguration config)
{
    public async Task<LoginResponse?> LoginAsync(string email, string password)
    {
        var usuario = await db.Usuarios
            .Include(u => u.Tenant)
            .FirstOrDefaultAsync(u => u.Email == email && u.Activo);

        if (usuario is null) return null;
        if (!BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash)) return null;

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Name, usuario.Nombre),
            new Claim(ClaimTypes.Role, usuario.Rol),
            new Claim("TenantId", usuario.TenantId.ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        return new LoginResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Nombre = usuario.Nombre,
            Rol = usuario.Rol,
            SuscripcionEstado = usuario.Tenant?.SuscripcionEstado
        };
    }
    public async Task<(RegistroResponse? result, string? error)> RegistrarAsync(RegistroRequest req)
    {
        // Verificar slug único
        var slugExiste = await db.Tenants.AnyAsync(t => t.Slug == req.Slug);
        if (slugExiste)
            return (null, "La URL ya está en uso, elegí otra");

        // Verificar email único
        var emailExiste = await db.Usuarios.AnyAsync(u => u.Email == req.AdminEmail);
        if (emailExiste)
            return (null, "Ya existe una cuenta con ese email");

        // Crear tenant
        var tenant = new Data.Entities.Tenant
        {
            Nombre = req.NombreNegocio,
            Slug = req.Slug.ToLower().Trim(),
            Telefono = req.Telefono,
            LogoUrl = req.LogoUrl,
            FotoPortada = req.FotoPortada,
            Descripcion = req.Descripcion,
            Direccion = req.Direccion,
            Instagram = req.Instagram,
            ColorPrimario = req.ColorPrimario ?? "#534AB7",
            Email = req.AdminEmail,
            Activo = true
        };
        db.Tenants.Add(tenant);
        await db.SaveChangesAsync();

        // Crear usuario admin
        var usuario = new Data.Entities.Usuario
        {
            TenantId = tenant.Id,
            Nombre = req.AdminNombre,
            Email = req.AdminEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.AdminPassword),
            Rol = "Admin",
            Activo = true
        };
        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();

        // Generar token directo
        var loginResult = await LoginAsync(req.AdminEmail, req.AdminPassword);
        if (loginResult is null) return (null, "Error al iniciar sesión");

        return (new RegistroResponse
        {
            Token = loginResult.Token,
            Nombre = loginResult.Nombre,
            Rol = loginResult.Rol,
            Slug = tenant.Slug
        }, null);
    }
}