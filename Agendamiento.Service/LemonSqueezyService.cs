using Agendamiento.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Agendamiento.Service;

public class LemonSqueezyService(AppDbContext db, IConfiguration config, HttpClient http)
{
    private readonly string _apiKey = config["LemonSqueezy:ApiKey"]!;
    private readonly string _storeId = config["LemonSqueezy:StoreId"]!;
    private readonly string _secret = config["LemonSqueezy:WebhookSecret"]!;

    private static readonly Dictionary<string, string> _variants = new()
    {
        { "Basic",      "1496908" },
        { "Individual", "1496923" },
        { "Premium",    "1496933" },
    };

    public async Task<string?> CrearCheckoutAsync(int tenantId, string plan, string email, string nombre)
    {
        if (!_variants.TryGetValue(plan, out var variantId))
            return null;

        var body = new
        {
            data = new
            {
                type = "checkouts",
                attributes = new
                {
                    checkout_data = new
                    {
                        email,
                        name = nombre,
                        custom = new { tenant_id = tenantId.ToString(), plan }
                    },
                    product_options = new
                    {
                        redirect_url = $"{config["App:FrontendUrl"]}/admin/dashboard?onboarding=true"
                    }
                },
                relationships = new
                {
                    store = new { data = new { type = "stores", id = _storeId } },
                    variant = new { data = new { type = "variants", id = variantId } }
                }
            }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.lemonsqueezy.com/v1/checkouts")
        {
            Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/vnd.api+json")
        };
        request.Headers.Add("Authorization", $"Bearer {_apiKey}");
        request.Headers.Add("Accept", "application/vnd.api+json");

        var response = await http.SendAsync(request);
        if (!response.IsSuccessStatusCode) return null;

        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return json.RootElement
            .GetProperty("data")
            .GetProperty("attributes")
            .GetProperty("url")
            .GetString();
    }

    public bool ValidarFirma(string payload, string signature)
    {
        var key = Encoding.UTF8.GetBytes(_secret);
        var data = Encoding.UTF8.GetBytes(payload);
        var hash = HMACSHA256.HashData(key, data);
        var computed = Convert.ToHexString(hash).ToLower();
        return computed == signature;
    }

    public async Task ProcesarWebhookAsync(string payload)
    {
        var doc = JsonDocument.Parse(payload);
        var root = doc.RootElement;
        var meta = root.GetProperty("meta");
        var eventName = meta.GetProperty("event_name").GetString();

        if (eventName is not ("subscription_created" or "subscription_updated" or "subscription_cancelled"))
            return;

        var data = root.GetProperty("data");
        var attributes = data.GetProperty("attributes");
        var customData = meta.GetProperty("custom_data");

        if (!customData.TryGetProperty("tenant_id", out var tenantIdEl)) return;
        if (!int.TryParse(tenantIdEl.GetString(), out var tenantId)) return;

        var tenant = await db.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId);
        if (tenant is null) return;

        var subscriptionId = data.GetProperty("id").GetString();
        var status = attributes.GetProperty("status").GetString();
        var customerId = attributes.GetProperty("customer_id").ToString();
        var plan = customData.TryGetProperty("plan", out var planEl) ? planEl.GetString() : null;

        DateTime? vence = null;
        if (attributes.TryGetProperty("renews_at", out var renovacionEl) &&
            DateTime.TryParse(renovacionEl.GetString(), out var renovacion))
            vence = renovacion;

        tenant.LemonSqueezySubscriptionId = subscriptionId;
        tenant.LemonSqueezyCustomerId = customerId;
        tenant.PlanNombre = plan;
        tenant.SuscripcionVence = vence;
        tenant.SuscripcionEstado = eventName == "subscription_cancelled" ? "cancelada"
                                          : status == "active" ? "activa"
                                          : status;

        await db.SaveChangesAsync();
    }
}