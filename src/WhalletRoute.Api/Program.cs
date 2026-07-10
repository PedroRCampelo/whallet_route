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

#region Service Registration

#region API Explorer & Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion

#region Database
var connectionString = builder.Configuration.GetConnectionString("Postgres")
                       ?? throw new InvalidOperationException("Missing connection string 'Postgres'.");

builder.Services.AddDbContext<WhalletRouteDbContext>(options =>
    options.UseNpgsql(connectionString));
#endregion

#region CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});
#endregion

#region Tenancy
builder.Services.AddScoped<ITenantResolver, DbTenantResolver>();
builder.Services.AddScoped<ITenantContext, TenantContext>();
#endregion

#region Routing
builder.Services.AddScoped<IRouteSolver, OrToolsRouteSolver>();
builder.Services.AddScoped<RouteOptimizationService>();
#endregion

#region Geocoding
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
#endregion

#endregion

var app = builder.Build();

#region Development-only Setup
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<WhalletRouteDbContext>();
    await DbSeeder.SeedAsync(db);

    app.UseSwagger();
    app.UseSwaggerUI();
}
#endregion

#region Middleware Pipeline
app.UseCors("DevCors");
app.UseMiddleware<ApiKeyMiddleware>();
#endregion

#region Endpoints
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
#endregion

app.Run();
