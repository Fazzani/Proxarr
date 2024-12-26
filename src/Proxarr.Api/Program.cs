using Proxarr.Api;
using Proxarr.Api.Core;
using Proxarr.Api.Services;
using Radarr.Http.Client;
using Scalar.AspNetCore;
using Serilog;
using Sonarr.Http.Client;
using TMDbLib.Client;


var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .Enrich.FromLogContext()
  .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton(x => new TMDbClient(Environment.GetEnvironmentVariable("TMDB_API_KEY")));
builder.Services.AddSingleton<AppConfiguration>();
builder.Services.AddTransient<ApiKeyDelegatingHandler>();
builder.Services.AddScoped<IRadarrService, RadarrService>();
builder.Services.AddScoped<ISonarrService, SonarrService>();

builder.Services
    .AddHttpClient<RadarrClient>()
    .AddHttpMessageHandler<ApiKeyDelegatingHandler>();

builder.Services
    .AddHttpClient<SonarrClient>()
    .AddHttpMessageHandler<ApiKeyDelegatingHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
