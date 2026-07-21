namespace WhalletRoute.Application.Cargos.Contracts;

public sealed record CreateDeliveryRequest
{
    public required string ExternalId { get; init; }
    public required string ClientId { get; init; }
    public string ClientName { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public double WeightKg { get; init; }
    public double VolumeM3 { get; init; }
    public TimeOnly? WindowFrom { get; init; }
    public TimeOnly? WindowTo { get; init; }
    public string? Phone { get; init; }
    public string? Instructions { get; init; }
}

public sealed record CreateCargoRequest
{
    public required string ExternalId { get; init; }
    public required string OriginAddress { get; init; }
    public double? OriginLatitude { get; init; }
    public double? OriginLongitude { get; init; }
    public Guid? DriverId { get; init; }
    public Guid? VehicleId { get; init; }
    public required IReadOnlyList<CreateDeliveryRequest> Deliveries { get; init; }
}

public sealed record DeliveryResponse
{
    public required Guid Id { get; init; }
    public required string ExternalId { get; init; }
    public required string ClientId { get; init; }
    public required string ClientName { get; init; }
    public required string Address { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public int? Order { get; init; }
    public required string Status { get; init; }
}

public sealed record CargoResponse
{
    public required Guid Id { get; init; }
    public required string ExternalId { get; init; }
    public required string OriginAddress { get; init; }
    public required string Status { get; init; }
    public Guid? DriverId { get; init; }
    public Guid? VehicleId { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required IReadOnlyList<DeliveryResponse> Deliveries { get; init; }
}

public sealed record CargoSummaryResponse
{
    public required Guid Id { get; init; }
    public required string ExternalId { get; init; }
    public required string Status { get; init; }
    public required int DeliveryCount { get; init; }
    public required DateTime CreatedAt { get; init; }
}

public sealed record RefuseDeliveryRequest
{
    public required string Reason { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
}

public sealed record DeliverRequest
{
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public string? Note { get; init; }
}