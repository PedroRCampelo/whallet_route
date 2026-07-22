namespace WhalletRoute.Application.Fleet.Contracts;

public sealed record CreateDriverRequest
{
    public required string Name { get; init; }
    public string? Document { get; init; }
    public string? Phone { get; init; }
    public string? LicenseNumber { get; init; }
    public string? LicenseCategory { get; init; }
    public DateOnly? LicenseExpiry { get; init; }
}

public sealed record DriverResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Document { get; init; }
    public string? Phone { get; init; }
    public string? LicenseNumber { get; init; }
    public string? LicenseCategory { get; init; }
    public DateOnly? LicenseExpiry { get; init; }
}

public sealed record CreateVehicleRequest
{
    public required string Plate { get; init; }
    public double CapacityKg { get; init; }
    public double CapacityM3 { get; init; }
    public string? Description { get; init; }
}

public sealed record VehicleResponse
{
    public required Guid Id { get; init; }
    public required string Plate { get; init; }
    public required double CapacityKg { get; init; }
    public required double CapacityM3 { get; init; }
    public string? Description { get; init; }
}

public sealed record AssignDriverRequest
{
    public required Guid DriverId { get; init; }
}

public sealed record AssignVehicleRequest
{
    public required Guid VehicleId { get; init; }
}