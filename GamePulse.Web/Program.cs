using GamePulse.Core.Entites;
using GamePulse.Core.Interfaces;
using GamePulse.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddScoped<IReleasesParser, SteamReleasesParser>();
builder.Services.AddScoped<IGameParser, SteamApiGameParser>();

var app = builder.Build();

app.MapGet("/", async (IGameParser parser, HttpContext context) =>
{
    List<Game> games = await parser.GetGamesFromApiAsync(10);

    await context.Response.WriteAsJsonAsync(games);
});

app.Run();
