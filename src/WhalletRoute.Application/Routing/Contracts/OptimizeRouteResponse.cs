namespace WhalletRoute.Application.Routing.Contracts;

public sealed record RoutePointResponse
{
    public required string Id { get; init; }
    public required double Latitude { get; init; }
    public required double Longitude { get; init; }
}

public sealed record OptimizeRouteResponse
{
    public required RoutePointResponse Origin { get; init; }
    public required int TotalDistanceMeters { get; init; }
    public required int TotalDurationSeconds { get; init; }
    public required IReadOnlyList<OptimizedStopResponse> Stops { get; init; }
}