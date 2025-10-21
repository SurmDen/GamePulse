using GamePulse.Core.Interfaces;
using GamePulse.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddScoped<IReleasesParser, SteamReleasesParser>();
builder.Services.AddScoped<IGameParser, SteamApiGameParser>();

var app = builder.Build();

app.MapGet("/", async (IReleasesParser parser, HttpContext context) =>
{
    List<long> Ids = await parser.GetReleaseGamesIdsAsync(11);

    await context.Response.WriteAsJsonAsync(Ids);
});

app.Run();
