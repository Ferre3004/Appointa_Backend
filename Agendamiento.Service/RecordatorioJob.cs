using Agendamiento.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Agendamiento.Service;

public class RecordatorioJob(
    AppDbContext db,
    IConfiguration config,
    ILogger<RecordatorioJob> logger)
{
    public async Task EnviarRecordatorio(int reservaId)
    {
        var reserva = await db.Reservas
            .Include(r => r.Servicio)
            .Include(r => r.Profesional)
            .Include(r => r.Tenant)
            .FirstOrDefaultAsync(r => r.Id == reservaId);

        if (reserva is null)
        {
            logger.LogWarning("Reserva {Id} no encontrada para recordatorio", reservaId);
            return;
        }

        if (reserva.Estado == "Cancelada")
        {
            logger.LogInformation("Reserva {Id} cancelada, se omite el recordatorio", reservaId);
            reserva.RecordatorioEstado = "Omitido";
            await db.SaveChangesAsync();
            return;
        }

        try
        {
            var sid = config["Twilio:AccountSid"]!;
            var authToken = config["Twilio:AuthToken"]!;
            var from = config["Twilio:FromNumber"]!;

            TwilioClient.Init(sid, authToken);

            var fecha = reserva.FechaHora.ToLocalTime();
            var mensaje = $"""
                Hola {reserva.ClienteNombre} 👋

                Te recordamos que mañana tenés un turno en *{reserva.Tenant.Nombre}*:

                📋 Servicio: {reserva.Servicio.Nombre}
                👤 Profesional: {reserva.Profesional.Nombre}
                📅 Fecha: {fecha:dddd dd 'de' MMMM}
                🕐 Hora: {fecha:HH:mm}

                Si necesitás cancelar, contactanos al {reserva.Tenant.Telefono}.

                ¡Hasta mañana! 🗓
                """;

            await MessageResource.CreateAsync(
                from: new Twilio.Types.PhoneNumber(from),
                to: new Twilio.Types.PhoneNumber($"whatsapp:{reserva.ClienteTelefono}"),
                body: mensaje
            );

            reserva.RecordatorioEstado = "Enviado";
            logger.LogInformation("Recordatorio enviado para reserva {Id}", reservaId);
        }
        catch (Exception ex)
        {
            reserva.RecordatorioEstado = "Error";
            logger.LogError(ex, "Error enviando recordatorio para reserva {Id}", reservaId);
        }

        await db.SaveChangesAsync();
    }
}