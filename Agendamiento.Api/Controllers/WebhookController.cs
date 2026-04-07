using Agendamiento.Service;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Agendamiento.Api.Controllers;

[ApiController]
[Route("api/webhooks")]
public class WebhookController(LemonSqueezyService lsService) : ControllerBase
{
    [HttpPost("lemonsqueezy")]
public async Task<IActionResult> LemonSqueezy()
{
    using var reader = new StreamReader(Request.Body);
    var payload      = await reader.ReadToEndAsync();

    var signature = Request.Headers["X-Signature"].FirstOrDefault() ?? "";
    if (!lsService.ValidarFirma(payload, signature))
        return Unauthorized("Firma inválida");

    await lsService.ProcesarWebhookAsync(payload);
    return Ok();
}
}