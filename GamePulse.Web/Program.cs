using GamePulse.Application.Interfaces;
using GamePulse.Core.Interfaces;
using GamePulse.Core.Interfaces.Repositories;
using GamePulse.Core.Interfaces.Services;
using GamePulse.Infrastructure.Extentions;
using GamePulse.Infrastructure.MessageBus;
using GamePulse.Infrastructure.Repositories;
using GamePulse.Infrastructure.Services;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddLogging();
builder.Services.AddHttpClient();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Game Pulse API",
        Version = "v1",
        Description = "API for analyzing game statistics and trends",
        Contact = new OpenApiContact
        {
            Name = "Denis Surmanidze",
            Email = "surmanidzedenis609@gmail.com"
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. 
                      Enter your token in the text input below.
                      Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
});

//my custom extentions
builder.Services.AddCustomDbContext(builder.Configuration);
// contains JWT Bearer validation, ITokenService and IUserRepository
builder.Services.AddCustomAuthenticationWithServices(builder.Configuration);

builder.Services.AddTransient<IGameRepository, GameRepository>();
builder.Services.AddTransient<IGenreRepository, GenreRepository>();

builder.Services.AddTransient<IDateParser, SteamApiDateParser>();
builder.Services.AddTransient<IPasswordHasher, SHA256Hasher>();

// parsing releases id's from html page
builder.Services.AddTransient<IReleasesParser, SteamReleasesParser>();
// getting game info by GameId from api
builder.Services.AddTransient<IGameParser, SteamApiGameParser>();

//added background service for message consuming
builder.Services.AddHostedService<KafkaConsumeService>();
// registered services for producing messages
builder.Services.AddSingleton<IMessageProduceService, KafkaProduceService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();

    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

app.UseResponseCaching();

app.UseCors(options =>
{
    options.AllowAnyHeader();
    options.AllowAnyMethod();
    options.AllowAnyOrigin();
});

app.MapControllers();

app.Run();