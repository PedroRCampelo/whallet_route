using WhalletRoute.Application.Routing;
using WhalletRoute.Application.Routing.Contracts;
using WhalletRoute.Infrastructure.Routing;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IRouteSolver, OrToolsRouteSolver>();
builder.Services.AddScoped<RouteOptimizationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapPost("/v1/routes/optimize", (OptimizeRouteRequest request, RouteOptimizationService service) =>
{
    try
    {
        var response = service.Optimize(request);
        return Results.Ok(response);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.Run();