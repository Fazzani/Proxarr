using Microsoft.Extensions.Http.Resilience;
using Polly;
using Proxarr.Api.Configuration;
using Proxarr.Api.Core;
using Proxarr.Api.HostedServices;
using Proxarr.Api.Services;
using Radarr.Http.Client;
using Scalar.AspNetCore;
using Serilog;
using Sonarr.Http.Client;
using System.Net;
using TMDbLib.Client;


var builder = WebApplication.CreateBuilder(args);

var configFileName = "config.yml";

#if DEBUG
configFileName = "config.local.yml";
#endif

// Load configuration

var configDirPath = Environment.GetEnvironmentVariable("CONFIG_PATH") ?? Directory.GetCurrentDirectory();
builder.Configuration
    .AddEnvironmentVariables()
    .AddYamlFile(Path.Combine(configDirPath, configFileName), optional: false, reloadOnChange: true);

// Add log configuration

var logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .Enrich.FromLogContext()
  .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(
        new MediaAddedJsonConverter());
});

builder.Services.AddHealthChecks();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton(x => new TMDbClient(builder.Configuration.GetValue<string>("AppConfiguration:TMDB_API_KEY")));
builder.Services.Configure<AppConfiguration>(builder.Configuration.GetSection(AppConfiguration.SECTION_NAME));
builder.Services.AddTransient<ApiKeyDelegatingHandler>();
builder.Services.AddScoped<IRadarrService, RadarrService>();
builder.Services.AddScoped<ISonarrService, SonarrService>();

// Add resilience strategy
HttpStatusCode[] retryStatus = [HttpStatusCode.InternalServerError, HttpStatusCode.ServiceUnavailable, HttpStatusCode.TooManyRequests];

Action<HttpStandardResilienceOptions> resilienceStragtegy = options =>
{
    // Customize retry
    options.Retry.ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
        .Handle<HttpRequestException>()
        .HandleResult(response => retryStatus.Any(x => x == response.StatusCode));
    options.Retry.MaxRetryAttempts = 3;

    // Customize attempt timeout
    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(5);
};

builder.Services
    .AddHttpClient<RadarrClient>()
    .AddHttpMessageHandler<ApiKeyDelegatingHandler>()
    .AddStandardResilienceHandler(resilienceStragtegy);

builder.Services
    .AddHttpClient<SonarrClient>()
    .AddHttpMessageHandler<ApiKeyDelegatingHandler>()
    .AddStandardResilienceHandler(resilienceStragtegy);

builder.Services.AddCronJob<FullScanHostedService>(c =>
{
    c.TimeZoneInfo = TimeZoneInfo.Local;
    c.CronExpression = Environment.GetEnvironmentVariable("FULL_SCAN_CRON") ?? builder.Configuration.GetValue<string>("AppConfiguration:FULL_SCAN_CRON")!;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

await app.RunAsync();