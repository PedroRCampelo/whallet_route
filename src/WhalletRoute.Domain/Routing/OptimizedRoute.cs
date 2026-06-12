namespace WhalletRoute.Domain.Routing;

public sealed record OptimizedRoute
{
    public required double TotalDistanceMeters { get; init; }
    public required double TotalDurationSeconds { get; init; }
    public required IReadOnlyList<OrderedStop> Stops { get; init; }
}