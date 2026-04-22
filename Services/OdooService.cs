using System.Text;
using System.Text.Json;
using Odoo_Api.Models;

namespace Odoo_Api.Services;

public class OdooService
{
    private readonly HttpClient _httpClient;
    private readonly string _odooUrl = "http://localhost:8085"; 

    public OdooService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DeskontuEmaitza> EgiaztatuDeskontuaAsync(string kodea)
    {
        var eskaera = new
        {
            jsonrpc = "2.0",
            method = "call",
            @params = new { kodea = kodea },
            id = Guid.NewGuid().ToString()
        };

        var edukia = new StringContent(JsonSerializer.Serialize(eskaera), Encoding.UTF8, "application/json");
        var erantzuna = await _httpClient.PostAsync($"{_odooUrl}/jatetxeko/egiaztatu_deskontua", edukia);

        if (erantzuna.IsSuccessStatusCode)
        {
            var jsonErantzuna = await erantzuna.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(jsonErantzuna);
            
            if (doc.RootElement.TryGetProperty("result", out var result))
            {
                return new DeskontuEmaitza
                {
                    ExistitzenDa = result.TryGetProperty("existitzen_da", out var ex) && ex.GetBoolean(),
                    Balioa = result.TryGetProperty("balioa", out var bal) ? bal.GetDouble() : 0
                };
            }
        }

        return new DeskontuEmaitza { ExistitzenDa = false, Balioa = 0 };
    }
}
