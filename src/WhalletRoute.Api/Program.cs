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
using WhalletRoute.Application.Cargos;
using WhalletRoute.Application.Cargos.Contracts;

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

#region Cargo
builder.Services.AddScoped<ICargoRepository, EfCargoRepository>();
builder.Services.AddScoped<CargoService>();
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

app.MapPost("/v1/cargos", async (CreateCargoRequest request, CargoService service, ITenantContext tenant, CancellationToken cancellationToken) =>
{
    try
    {
        var response = await service.CreateAsync(tenant.Current!.Id, request, cancellationToken);
        return Results.Created($"/v1/cargos/{response.Id}", response);
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

app.MapGet("/v1/cargos", async (CargoService service, ITenantContext tenant, CancellationToken cancellationToken) =>
    Results.Ok(await service.ListAsync(tenant.Current!.Id, cancellationToken)));

app.MapGet("/v1/cargos/{id:guid}", async (Guid id, CargoService service, ITenantContext tenant, CancellationToken cancellationToken) =>
{
    var cargo = await service.GetAsync(tenant.Current!.Id, id, cancellationToken);
    return cargo is null ? Results.NotFound() : Results.Ok(cargo);
});

app.MapPost("/v1/cargos/{id:guid}/route", async (Guid id, CargoService service, ITenantContext tenant, CancellationToken cancellationToken) =>
{
    try
    {
        var cargo = await service.RouteAsync(tenant.Current!.Id, id, cancellationToken);
        return cargo is null ? Results.NotFound() : Results.Ok(cargo);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPost("/v1/cargos/{id:guid}/dispatch", async (Guid id, CargoService service, ITenantContext tenant, CancellationToken cancellationToken) =>
{
    try
    {
        var cargo = await service.DispatchAsync(tenant.Current!.Id, id, cancellationToken);
        return cargo is null ? Results.NotFound() : Results.Ok(cargo);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPost("/v1/cargos/{id:guid}/cancel-dispatch", async (Guid id, CargoService service, ITenantContext tenant, CancellationToken cancellationToken) =>
{
    try
    {
        var cargo = await service.CancelDispatchAsync(tenant.Current!.Id, id, cancellationToken);
        return cargo is null ? Results.NotFound() : Results.Ok(cargo);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPost("/v1/cargos/{cargoId:guid}/deliveries/{deliveryId:guid}/deliver", async (Guid cargoId, Guid deliveryId, DeliverRequest request, CargoService service, ITenantContext tenant, CancellationToken cancellationToken) =>
{
    try
    {
        var cargo = await service.DeliverAsync(tenant.Current!.Id, cargoId, deliveryId, request, cancellationToken);
        return cargo is null ? Results.NotFound() : Results.Ok(cargo);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPost("/v1/cargos/{cargoId:guid}/deliveries/{deliveryId:guid}/refuse", async (Guid cargoId, Guid deliveryId, RefuseDeliveryRequest request, CargoService service, ITenantContext tenant, CancellationToken cancellationToken) =>
{
    try
    {
        var cargo = await service.RefuseAsync(tenant.Current!.Id, cargoId, deliveryId, request, cancellationToken);
        return cargo is null ? Results.NotFound() : Results.Ok(cargo);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

#endregion

app.Run();
