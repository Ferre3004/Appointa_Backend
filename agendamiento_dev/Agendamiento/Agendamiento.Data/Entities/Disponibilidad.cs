namespace Agendamiento.Data.Entities;

public class Disponibilidad
{
    public int Id { get; set; }
    public int ProfesionalId { get; set; }
    public short DiaSemana { get; set; }  // 0=Dom, 1=Lun ... 6=Sab
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFin { get; set; }
    public bool Activo { get; set; } = true;

    public Profesional Profesional { get; set; } = null!;
}