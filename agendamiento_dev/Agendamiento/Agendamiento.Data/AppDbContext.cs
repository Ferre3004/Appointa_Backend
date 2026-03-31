using Agendamiento.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Agendamiento.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Profesional> Profesionales => Set<Profesional>();
    public DbSet<Servicio> Servicios => Set<Servicio>();
    public DbSet<Disponibilidad> Disponibilidades => Set<Disponibilidad>();
    public DbSet<Reserva> Reservas => Set<Reserva>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tenant>(e => {
    e.Property(x => x.ColorPrimario).HasColumnName("color_primario");
    e.Property(x => x.LogoUrl).HasColumnName("logo_url");
    e.Property(x => x.CreadoEn).HasColumnName("creado_en");
    e.Property(x => x.FotoPortada).HasColumnName("foto_portada");   
                });
        modelBuilder.Entity<Usuario>().ToTable("usuarios");
        modelBuilder.Entity<Profesional>().ToTable("profesionales");
        modelBuilder.Entity<Servicio>().ToTable("servicios");
        modelBuilder.Entity<Disponibilidad>().ToTable("disponibilidad");
        modelBuilder.Entity<Reserva>().ToTable("reservas");
    }
}