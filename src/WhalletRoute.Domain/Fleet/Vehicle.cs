namespace WhalletRoute.Domain.Fleet;

public sealed class Vehicle
{
    public Guid Id { get; }
    public string TenantId { get; }
    public string Plate { get; }
    public double CapacityKg { get; }
    public double CapacityM3 { get; }
    public string? Description { get; }

    public Vehicle(
        string tenantId, string plate,
        double capacityKg = 0, double capacityM3 = 0, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
            throw new ArgumentException("Tenant id is required.", nameof(tenantId));
        if (string.IsNullOrWhiteSpace(plate))
            throw new ArgumentException("Plate is required.", nameof(plate));

        Id = Guid.NewGuid();
        TenantId = tenantId;
        Plate = plate;
        CapacityKg = capacityKg;
        CapacityM3 = capacityM3;
        Description = description;
    }
}