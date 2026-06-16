using WhalletRoute.Application.Routing.Contracts;
using WhalletRoute.Domain.Geo;
using WhalletRoute.Domain.Routing;

namespace WhalletRoute.Application.Routing;

public sealed class RouteOptimizationService
{
    private readonly IRouteSolver _solver;

    public RouteOptimizationService(IRouteSolver solver)
    {
        _solver = solver;
    }

    public OptimizeRouteResponse Optimize(OptimizeRouteRequest request)
    {
        var origin = ToStop(request.Origin);
        var stops = request.Stops.Select(ToStop).ToList();

        if (stops.Count == 0)
            throw new ArgumentException("At least one stop is required.");

        var route = _solver.Solve(origin, stops);

        return new OptimizeRouteResponse
        {
            TotalDistanceMeters = (int)Math.Round(route.TotalDistanceMeters),
            TotalDurationSeconds = (int)Math.Round(route.TotalDurationSeconds),
            Stops = route.Stops
                .Select(stop => new OptimizedStopResponse
                {
                    Id = stop.StopId,
                    Order = stop.Order,
                    LegDistanceMeters = (int)Math.Round(stop.LegDistanceMeters),
                    LegDurationSeconds = (int)Math.Round(stop.LegDurationSeconds)
                })
                .ToList()
        };
    }

    private static Stop ToStop(StopRequest request)
    {
        if (request.Latitude is null || request.Longitude is null)
            throw new ArgumentException(
                $"Stop '{request.Id}' must include latitude and longitude. Address geocoding is not supported yet.");

        var coordinate = new Coordinate(request.Latitude.Value, request.Longitude.Value);
        return new Stop(request.Id, coordinate);
    }
}