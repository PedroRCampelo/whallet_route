namespace WhalletRoute.Domain.Fleet;

public sealed class Vehicle
{
    public Guid Id { get; private set; }
    public string TenantId { get; private set; } = null!;
    public string Plate { get; private set; } = null!;
    public double CapacityKg { get; private set; }
    public double CapacityM3 { get; private set; }
    public string? Description { get; private set; }
    
    private Vehicle() { }
    
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