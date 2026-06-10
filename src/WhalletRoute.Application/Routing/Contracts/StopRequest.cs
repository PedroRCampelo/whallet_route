namespace WhalletRoute.Application.Routing.Contracts;

public sealed record StopRequest
{
    public required string Id { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public string? Address { get; init; }
    public string? PostalCode { get; init; }
}