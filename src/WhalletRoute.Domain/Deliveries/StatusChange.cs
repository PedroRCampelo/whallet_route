using WhalletRoute.Domain.Geo;

namespace WhalletRoute.Domain.Deliveries;

public sealed record StatusChange
{
    public required DeliveryStatus To { get; init; }
    public required DateTime OccurredAt { get; init; }
    public Coordinate? Location { get; init; }
    public string? Note { get; init; }
}