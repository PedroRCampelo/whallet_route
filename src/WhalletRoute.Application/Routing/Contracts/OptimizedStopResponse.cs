namespace WhalletRoute.Application.Routing.Contracts;

public sealed record OptimizedStopResponse
{
    public required string Id { get; init; }
    public required int Order { get; init; }
    public required double Latitude { get; init; }
    public required double Longitude { get; init; }
    public required int LegDistanceMeters { get; init; }
    public required int LegDurationSeconds { get; init; }
}