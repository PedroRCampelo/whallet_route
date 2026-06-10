namespace WhalletRoute.Application.Routing.Contracts;

public sealed record OptimizeRouteRequest
{
    public required StopRequest Origin { get; init; }
    public required IReadOnlyList<StopRequest> Stops { get; init; }
}