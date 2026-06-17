using WhalletRoute.Application.Geocoding;
using WhalletRoute.Application.Routing.Contracts;
using WhalletRoute.Domain.Geo;
using WhalletRoute.Domain.Routing;

namespace WhalletRoute.Application.Routing;

public sealed class RouteOptimizationService
{
    private readonly IRouteSolver _solver;
    private readonly IGeocoder _geocoder;

    public RouteOptimizationService(IRouteSolver solver, IGeocoder geocoder)
    {
        _solver = solver;
        _geocoder = geocoder;
    }

    public async Task<OptimizeRouteResponse> OptimizeAsync(OptimizeRouteRequest request, CancellationToken cancellationToken)
    {
        var origin = await ToStopAsync(request.Origin, cancellationToken);

        var stops = new List<Stop>();
        foreach (var stopRequest in request.Stops)
            stops.Add(await ToStopAsync(stopRequest, cancellationToken));

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

    private async Task<Stop> ToStopAsync(StopRequest request, CancellationToken cancellationToken)
    {
        if (request.Latitude is not null && request.Longitude is not null)
        {
            var explicitCoordinate = new Coordinate(request.Latitude.Value, request.Longitude.Value);
            return new Stop(request.Id, explicitCoordinate);
        }

        if (!string.IsNullOrWhiteSpace(request.Address))
        {
            var geocoded = await _geocoder.GeocodeAsync(request.Address, cancellationToken);
            return new Stop(request.Id, geocoded);
        }

        throw new ArgumentException($"Stop '{request.Id}' must include either coordinates or an address.");
    }
}