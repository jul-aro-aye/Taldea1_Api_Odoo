using Odoo_Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<OdooService>();

var app = builder.Build();

app.MapGet("/", () => "Odoo API martxan dago!");

app.MapPost("/api/deskontuak/egiaztatu", async (DeskontuEskaera eskaera, OdooService odooService) =>
{
    var emaitza = await odooService.EgiaztatuDeskontuaAsync(eskaera.kodea);
    return Results.Ok(emaitza);
});

app.Run();

public record DeskontuEskaera(string kodea);
