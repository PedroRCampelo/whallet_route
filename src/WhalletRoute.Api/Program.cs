using WhalletRoute.Application.Geocoding;
using WhalletRoute.Application.Routing;
using WhalletRoute.Application.Routing.Contracts;
using WhalletRoute.Infrastructure.Geocoding;
using WhalletRoute.Infrastructure.Routing;
using WhalletRoute.Api.Authentication;
using WhalletRoute.Application.Tenancy;
using WhalletRoute.Infrastructure.Tenancy;
using Microsoft.EntityFrameworkCore;
using WhalletRoute.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("Postgres")
                       ?? throw new InvalidOperationException("Missing connection string 'Postgres'.");

builder.Services.AddDbContext<WhalletRouteDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddSingleton<ITenantResolver, InMemoryTenantResolver>();
builder.Services.AddScoped<ITenantContext, TenantContext>();

builder.Services.AddScoped<IRouteSolver, OrToolsRouteSolver>();
builder.Services.AddSingleton<IGeocodeCache, InMemoryGeocodeCache>();

var googleApiKey = builder.Configuration["Google:GeocodingApiKey"]
    ?? throw new InvalidOperationException("Missing configuration 'Google:GeocodingApiKey'.");

builder.Services.AddHttpClient<IGeocoder, GoogleGeocoder>()
    .ConfigureHttpClient(client => client.Timeout = TimeSpan.FromSeconds(10));

builder.Services.AddScoped<IGeocoder>(sp =>
{
    var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(GoogleGeocoder));
    var cache = sp.GetRequiredService<IGeocodeCache>();
    return new GoogleGeocoder(httpClient, cache, googleApiKey);
});

builder.Services.AddScoped<RouteOptimizationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("DevCors");

app.UseMiddleware<ApiKeyMiddleware>();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapPost("/v1/routes/optimize", async (OptimizeRouteRequest request, RouteOptimizationService service, CancellationToken cancellationToken) =>
{
    try
    {
        var response = await service.OptimizeAsync(request, cancellationToken);
        return Results.Ok(response);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.Run();