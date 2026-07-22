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
using WhalletRoute.Application.Fleet;
using WhalletRoute.Application.Fleet.Contracts;

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

#region Driver / Vehicle
builder.Services.AddScoped<IDriverRepository, EfDriverRepository>();
builder.Services.AddScoped<IVehicleRepository, EfVehicleRepository>();
builder.Services.AddScoped<FleetService>();
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

app.MapPost("/v1/drivers", async (CreateDriverRequest request, FleetService service, ITenantContext tenant, CancellationToken cancellationToken) =>
{
    try
    {
        var driver = await service.CreateDriverAsync(tenant.Current!.Id, request, cancellationToken);
        return Results.Created($"/v1/drivers/{driver.Id}", driver);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapGet("/v1/drivers", async (FleetService service, ITenantContext tenant, CancellationToken cancellationToken) =>
    Results.Ok(await service.ListDriversAsync(tenant.Current!.Id, cancellationToken)));

app.MapGet("/v1/drivers/{id:guid}", async (Guid id, FleetService service, ITenantContext tenant, CancellationToken cancellationToken) =>
{
    var driver = await service.GetDriverAsync(tenant.Current!.Id, id, cancellationToken);
    return driver is null ? Results.NotFound() : Results.Ok(driver);
});

app.MapPost("/v1/vehicles", async (CreateVehicleRequest request, FleetService service, ITenantContext tenant, CancellationToken cancellationToken) =>
{
    try
    {
        var vehicle = await service.CreateVehicleAsync(tenant.Current!.Id, request, cancellationToken);
        return Results.Created($"/v1/vehicles/{vehicle.Id}", vehicle);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapGet("/v1/vehicles", async (FleetService service, ITenantContext tenant, CancellationToken cancellationToken) =>
    Results.Ok(await service.ListVehiclesAsync(tenant.Current!.Id, cancellationToken)));

app.MapGet("/v1/vehicles/{id:guid}", async (Guid id, FleetService service, ITenantContext tenant, CancellationToken cancellationToken) =>
{
    var vehicle = await service.GetVehicleAsync(tenant.Current!.Id, id, cancellationToken);
    return vehicle is null ? Results.NotFound() : Results.Ok(vehicle);
});

app.MapPost("/v1/cargos/{id:guid}/assign-driver", async (Guid id, AssignDriverRequest request, CargoService service, ITenantContext tenant, CancellationToken cancellationToken) =>
{
    try
    {
        var cargo = await service.AssignDriverAsync(tenant.Current!.Id, id, request.DriverId, cancellationToken);
        return cargo is null ? Results.NotFound() : Results.Ok(cargo);
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

app.MapPost("/v1/cargos/{id:guid}/assign-vehicle", async (Guid id, AssignVehicleRequest request, CargoService service, ITenantContext tenant, CancellationToken cancellationToken) =>
{
    try
    {
        var cargo = await service.AssignVehicleAsync(tenant.Current!.Id, id, request.VehicleId, cancellationToken);
        return cargo is null ? Results.NotFound() : Results.Ok(cargo);
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
