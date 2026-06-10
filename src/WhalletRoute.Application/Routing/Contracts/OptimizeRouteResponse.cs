namespace WhalletRoute.Application.Routing.Contracts;

public sealed record OptimizeRouteResponse
{
    public required int TotalDistanceMeters { get; init; }
    public required int TotalDurationSeconds { get; init; }
    public required IReadOnlyList<OptimizedStopResponse> Stops { get; init; }
}