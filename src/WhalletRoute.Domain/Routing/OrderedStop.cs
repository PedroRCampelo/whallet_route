namespace WhalletRoute.Domain.Routing;

public sealed record OrderedStop
{
    public required string StopId { get; init; }
    public required int Order { get; init; }
    public required double LegDistanceMeters { get; init; }
    public required double LegDurationSeconds { get; init; }
}